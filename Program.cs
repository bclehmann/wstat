using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using Where1.wstat.Graph;
using Where1.wstat.Regression;
using Where1.wstat.Distribution;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Where1.wstat
{
	class Program
	{
		private static readonly Dictionary<string, string> Shortcuts = new Dictionary<string, string>(){
			{ "list", "operation=list" },
			{ "summary", "operation=summary" },
			{ "json", "output=json" },
			{ "text", "output=text" },
			{ "csv", "output=csv" },
			{ "2var", "dimensions=2" },
			{ "plot", "operation=plot" },
			{ "linreg", "options=linreg" },
			{ "reexpress", "operation=reexpress" },
			{ "zscore", "options=zscore"},
			{ "population", "options=population"},
			{ "sample", "options=sample"},
			{ "residual", "options=residual"},
			{ "correlate", "operation=correlation"},
			{ "correlation", "operation=correlation"},
			{ "cdf", "operation=cdf"},
			{ "invcdf", "operation=invcdf"},
			{ "quantile", "operation=quantile"}
		};

		private static readonly Dictionary<string, Operation> OperationDictionary = new Dictionary<string, Operation>() {
			{ "summary", Operation.summary },
			{ "list", Operation.list },
			{ "plot", Operation.plot },
			{ "reexpress", Operation.reexpress },
			{ "correlation", Operation.correlation},
			{ "cdf", Operation.cdf},
			{ "invcdf", Operation.invCdf},
			{ "quantile", Operation.quantile}
		};

		private static readonly Dictionary<string, Output> OutputDictionary = new Dictionary<string, Output>() {
			{ "text", Output.text },
			{ "json", Output.json },
			{ "csv", Output.csv },
		};

		private static readonly Dictionary<string, string> MultiSpaceFlags = new Dictionary<string, string>(){
			{ "-o", "file=" }
		};


		private const RegexOptions options = RegexOptions.Multiline;
		private const string set_pattern = @"set=(.+)";
		private const string operation_pattern = @"operation=(.+)";
		private const string output_pattern = @"output=(.+)";
		private const string dimension_pattern = @"dimensions=(\d+)";
		private const string options_pattern = @"options=(.+)";
		private const string fileOut_pattern = @"file=([\w\/\\:~.*\d]+)";
		private const string filePathPattern = @"([\w\/\\:~\d"".])+";
		private const string letterPattern = @"[a-zA-Z]";
		private const string scientificNotationPattern = @"(-?\d*.?\d+)[eE](-?\d)";
		private const string listScientificNotationPattern = "((" + scientificNotationPattern + "),?)+";//Yeah, I hate it too
		private const string quantileRankPattern = @"rank=(\d+)";


		//These two are to avoid filepaths with spaces being split up into multiple arguments
		private static string FilePathEncode(string input) => input.Replace(" ", "*20");
		private static string FilePathDecode(string input) => input.Replace("*20", " ");

		private static string GetRegexGroup(string input, string pattern)
		{
			string output = "";

			foreach (Match m in Regex.Matches(input, pattern, options))
			{
				int i = 0;
				foreach (var g in m.Groups)
				{
					if (i != 0)
					{
						output += g.ToString();
					}
					i++;
				}
			}

			return output != "" ? output : null;
		}

		private static bool AskIfPopulation()
		{
			Console.WriteLine("Is your set a population? (Y/N)\n\nIf you don't know, select \"No\"");
			return Console.ReadLine().ToUpper() == "Y";
		}

		private static void Spinner()
		{
			int i = 0;
			char[] spinChar = { '/', '-', '\\', '|' };
			int numberOfColours = Enum.GetNames(typeof(ConsoleColor)).Length;
			do
			{
				if (i > 400)
				{
					break;
				}
				Console.ForegroundColor = (ConsoleColor)(i % numberOfColours);
				Console.Write(spinChar[i % spinChar.Length]);
				if (i % 8 == 0)
				{
					Console.Write(" ");
				}
				Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
				Thread.Sleep(200);
				i++;
			} while (!Console.KeyAvailable);

			Console.ReadKey();
			Console.ResetColor();
		}

		private static void Print<T>(T toPrint, bool printToFileToo, StreamWriter outputStream)
		{
			Console.Write(toPrint);
			if (printToFileToo)
			{
				outputStream.Write(toPrint);
				outputStream.Flush();
			}
		}

		private static void PrintLine<T>(T toPrint, bool printToFileToo, StreamWriter outputStream)
		{
			Print(toPrint + "\n", printToFileToo, outputStream);
		}

		private static string LinRegPrintout(VectorSet vset, Output outputFormat = Output.text)
		{
			IRegressionLine regline = new LinearRegressionLine();
			double[] coefficients = regline.Calculate(vset);
			double r_squared = regline.CoefficientOfDetermination(vset);

			switch (outputFormat)
			{
				case Output.text:
					string returnVal = $"\n" +
						$"\ty=b0 + b1x1 + b2x2 + ... + bnxn" +
						$"\n\n";

					for (int i = 0; i < coefficients.Length; i++)
					{
						returnVal += $"\tb{i} = {coefficients[i]}\n";
					}

					returnVal += $"\n\n\tCoefficient of Determination (r^2) = {r_squared}";

					return returnVal;
					break;
				case Output.json:
					return JsonSerializer.Serialize(new
					{
						Coefficients = coefficients,
						CoefficientOfDetermination = r_squared
					});
					break;
				case Output.csv:
					string output = "";
					for (int i = 0; i < coefficients.Length; i++)
					{
						output += coefficients[i];
						if (i != coefficients.Length - 1)
						{
							output += ",";
						}
					}
					return output;
					break;
				default:
					throw new NotSupportedException("This function does not support the specified output format");
			}
		}

		public static void Main(string[] args)
		{
			string[] expandedArgs = new string[args.Length];
			for (int i = 0; i < args.Length; i++)
			{
				if (MultiSpaceFlags.ContainsKey(args[i].ToLower()))
				{
					expandedArgs[i] = MultiSpaceFlags.GetValueOrDefault(args[i].ToLower()).Trim() + args[i + 1];
					args[i + 1] = "";
				}
				else if (Shortcuts.ContainsKey(args[i].ToLower()))
				{
					expandedArgs[i] = Shortcuts.GetValueOrDefault(args[i].ToLower());
				}
				else
				{
					expandedArgs[i] = args[i];
				}
				expandedArgs[i] = FilePathEncode(expandedArgs[i]);
			}

			Run(expandedArgs);
		}

		async private static void Run(string[] args)
		{
			try
			{
				StringBuilder setRaw = new StringBuilder();
				Operation operation = Operation.list;
				Output output = Output.text;
				int dimensions = 1;
				List<string> enabledOptions = new List<string>();
				double? normalDistributionParameter = null;
				string outputFilePath = "";
				bool writeToFile = false;
				StreamWriter outputStream = null;
				bool? isPopulation = null;
				int quantileRank = 100;

				foreach (string curr in args)
				{
					setRaw.Append(GetRegexGroup(curr, set_pattern));

					string operationName = GetRegexGroup(curr, operation_pattern);
					if (operationName != null)
					{
						Operation tempOperation;
						if (OperationDictionary.TryGetValue(operationName, out tempOperation))
						{
							operation = tempOperation;
						}
					}

					string outputName = GetRegexGroup(curr, output_pattern);
					if (outputName != null)
					{
						Output tempOutput;
						if (OutputDictionary.TryGetValue(outputName, out tempOutput))
						{
							output = tempOutput;
						}
					}

					int tempDimensions;
					if (int.TryParse(GetRegexGroup(curr, dimension_pattern), out tempDimensions))
					{
						dimensions = tempDimensions;
					}

					enabledOptions.Add(GetRegexGroup(curr, options_pattern));

					string filename = GetRegexGroup(curr, fileOut_pattern);
					if (filename != null)
					{
						outputFilePath = filename;
						writeToFile = true;
					}

					int tempQuantileRank;
					if (int.TryParse(GetRegexGroup(curr, quantileRankPattern), out tempQuantileRank))
					{
						quantileRank = tempQuantileRank;
					}
				}

				if (enabledOptions.Contains("sample"))
				{
					isPopulation = false;
				}
				else if (enabledOptions.Contains("population"))
				{
					isPopulation = true;
				}

				if (writeToFile)
				{
					Console.WriteLine("Output redirected to: " + outputFilePath); //This is intentially printed to console ONLY (keeps JSON/CSV output files parsable)
					outputStream = new StreamWriter(outputFilePath);
				}

				using (outputStream)
				{

					switch (operation)
					{
						case Operation.cdf:
							if (!normalDistributionParameter.HasValue)
							{
								Console.WriteLine("What is the z-score?");
								normalDistributionParameter = double.Parse(Console.ReadLine());
							}

							PrintLine(NormalDistribution.Cdf(normalDistributionParameter.Value), writeToFile, outputStream);
							return;
							break;
						case Operation.invCdf:
							if (!normalDistributionParameter.HasValue)
							{
								Console.WriteLine("What is the probability?");
								normalDistributionParameter = double.Parse(Console.ReadLine());
							}

							PrintLine(NormalDistribution.InvCdf(normalDistributionParameter.Value), writeToFile, outputStream);
							return;
							break;
						default:
							break;
					}

					if (setRaw.Length == 0)
					{
						Console.WriteLine("\nType your set here:");
						setRaw.Append(Console.ReadLine());
						Console.WriteLine($"\nYour set:\n" +
											$"{setRaw}\n");
					}

					string setStringPath = FilePathDecode(setRaw.ToString());
					setStringPath = setStringPath.Replace("\"", "");

					//If it is a valid path, it contains letters, and it contains letters for a reason other than it being in scientific notation
					if (Regex.IsMatch(setStringPath.ToString(), filePathPattern) && Regex.IsMatch(setStringPath.ToString(), letterPattern) && !Regex.IsMatch(setStringPath.ToString(), listScientificNotationPattern))
					{
						StreamReader reader = new StreamReader(setStringPath);
						setRaw.Clear();
						setRaw.Append(reader.ReadToEnd());
						reader.Close();
						reader.Dispose();
					}
					//double.Parse(s) actually can handle scientific notation, so my work here is actually done

					List<string> setStringList = setRaw.Replace("(", "")
														.Replace(")", "")
														.Replace(" ", "")
														.Replace("\n", "")
														.ToString()
														.Split(',')
														.ToList();

					if (dimensions == 1)
					{
						DataSet set = new DataSet(setStringList.Select(s => double.Parse(s)).ToList());
						switch (operation)
						{
							case Operation.list://Default
								Print(set.List(output), writeToFile, outputStream);
								break;
							case Operation.summary:
								Print(set.Summarize(output), writeToFile, outputStream);
								break;
							case Operation.reexpress:
								if (enabledOptions.Contains("zscore"))
								{
									if (!isPopulation.HasValue)
									{
										isPopulation = AskIfPopulation();
									}

									Print(set.StandardizeSet(isPopulation.Value).List(output), writeToFile, outputStream);
								}
								break;
							case Operation.quantile:
								Print(set.Quantile(quantileRank).List(output), writeToFile, outputStream);
								break;
						}
					}
					else
					{
						VectorSet vectorSet;
						List<double> unwoundSet = new List<double>();
						unwoundSet.AddRange(setStringList.Select(s => double.Parse(s)).ToList());

						vectorSet = VectorSet.CreateVectorSetFromList(unwoundSet, dimensions);

						switch (operation)
						{
							case Operation.list: //Default
								Print("\n\n", writeToFile, outputStream);
								if (enabledOptions.Contains("linreg"))
								{
									PrintLine(LinRegPrintout(vectorSet, output), writeToFile, outputStream);
								}
								else
								{
									Print(vectorSet.List(output), writeToFile, outputStream);
								}
								break;
							case Operation.summary:
								Print(vectorSet.Summarize(output), writeToFile, outputStream);
								break;
							case Operation.quantile:
								switch (output)
								{
									case Output.text:
										string outputString = "";
										for (int i = 0; i < vectorSet.Dimensions; i++)
										{
											outputString += $"\tDimension {i + 1} ({vectorSet.Dimensions} total):\n\n";
											outputString += vectorSet.DataSets[i].Quantile(quantileRank).List(output);
										}
										Print(outputString, writeToFile, outputStream);
										break;
									case Output.json:
										List<List<double>> dataSetList = new List<List<double>>(vectorSet.Dimensions);
										for (int i = 0; i < vectorSet.Dimensions; i++)
										{
											dataSetList.Add(vectorSet.DataSets[i].Quantile(quantileRank).GetSet());
										}
										Print(JsonSerializer.Serialize(dataSetList), writeToFile, outputStream);
										break;
									default:
										throw new NotSupportedException("This operation does not support this output format");
								}
								break;
							case Operation.plot:
								Plot plot = new Plot(vectorSet);
								string filename = "";
								if (enabledOptions.Contains("linreg"))
								{
									filename = await plot.Draw(RegressionLines.linear);
									PrintLine(LinRegPrintout(vectorSet, output), writeToFile, outputStream);
								}
								else
								{
									filename = await plot.Draw();
								}

								PrintLine($"\n\tFilepath: {filename}", writeToFile, outputStream);
								if (RuntimeInformation.OSDescription.ToLower().Contains("windows"))
								{
									string strCmdText = "/C \"" + filename + "\"";
									System.Diagnostics.Process.Start("CMD.exe", strCmdText);
								}
								else
								{
									string strCmdText = "xdg-open " + filename;
									System.Diagnostics.Process.Start("bash", strCmdText);
								}
								break;
							case Operation.reexpress:
								if (enabledOptions.Contains("zscore"))
								{
									if (!isPopulation.HasValue)
									{
										isPopulation = AskIfPopulation();
									}

									Print(vectorSet.StandardizeSet(isPopulation.Value).List(output), writeToFile, outputStream);
								}
								else if (enabledOptions.Contains("residual"))
								{
									if (enabledOptions.Contains("linreg"))
									{
										Print(vectorSet.ResidualSet(new LinearRegressionLine()).List(output), writeToFile, outputStream);
									}
								}
								break;
							case Operation.correlation:
								if (enabledOptions.Contains("population"))
								{
									isPopulation = true;
								}
								else if (enabledOptions.Contains("sample"))
								{
									isPopulation = false;
								}
								else
								{
									isPopulation = AskIfPopulation();
								}

								Print(vectorSet.Correlation(isPopulation.Value), writeToFile, outputStream);
								break;
						}
					}

					if (outputStream != null)
					{
						if (isPopulation.HasValue && output == Output.text)
						{
							outputStream.WriteLine();
							outputStream.WriteLine();
							outputStream.WriteLine($"You specified that this {(isPopulation.Value ? "was" : "was not")} a population.");
						}
					}
				}
			}
			catch (Exception e)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.BackgroundColor = ConsoleColor.Black;

				Console.WriteLine("The program ran into an error:");
				Console.WriteLine("=======================================================================");
				Console.WriteLine($"Exception type: {e.GetType().Name}");
				Console.WriteLine($"Exception fully-qualified type: {e.GetType().ToString()}");
				Console.WriteLine("Exception message:");
				Console.WriteLine($"\t{e.Message}\n");
				Console.WriteLine("Stacktrace:");
				Console.WriteLine(e.StackTrace);

				Console.ResetColor();
				Console.WriteLine("\n\nPress any key to exit");

				Spinner();
			}
		}
	}
}
