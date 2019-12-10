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

namespace Where1.wstat
{
    class Program
    {
        private static Dictionary<string, string> Shortcuts = new Dictionary<string, string>(){
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
        };

        private static Dictionary<string, Operation> OperationDictionary = new Dictionary<string, Operation>() {
            { "summary", Operation.summary },
            { "list", Operation.list },
            { "plot", Operation.plot },
            { "reexpress", Operation.reexpress },
            { "correlation", Operation.correlation},
            { "cdf", Operation.cdf},
            { "invcdf", Operation.invCdf},
        };

        private static Dictionary<string, Output> OutputDictionary = new Dictionary<string, Output>() {
            { "text", Output.text },
            { "json", Output.json },
            { "csv", Output.csv },
        };

        private static Dictionary<string, string> MultiSpaceFlags = new Dictionary<string, string>(){
            { "-o", "file=" }
        };

        public static void Main(string[] args)
        {
            Run(args);
        }

        public static string FilePathEncode(string input) => input.Replace(" ", "*20");
        public static string FilePathDecode(string input) => input.Replace("*20", " ");
        async static void Run(string[] args)
        {
            const string set_pattern = @"set=(.+)";
            const string operation_pattern = @"operation=(.+)";
            const string output_pattern = @"output=(.+)";
            const string dimension_pattern = @"dimensions=(\d+)";
            const string options_pattern = @"options=(.+)";
            const string fileOut_pattern = @"file=([\w\/\\:~.*\d]+)";



            RegexOptions options = RegexOptions.Multiline;

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



            foreach (string curr in expandedArgs)
            {
                foreach (Match m in Regex.Matches(curr, set_pattern, options))
                {
                    int i = 0;
                    foreach (var g in m.Groups)
                    {
                        if (i != 0)
                        {
                            setRaw.Append(g);
                        }
                        i++;
                    }
                }

                foreach (Match m in Regex.Matches(curr, operation_pattern, options))
                {
                    int i = 0;
                    foreach (var g in m.Groups)
                    {
                        if (i != 0)
                        {
                            Operation tempOperation;
                            if (OperationDictionary.TryGetValue((string)g.ToString(), out tempOperation))
                            {
                                operation = tempOperation;
                            }
                        }
                        i++;
                    }
                }

                foreach (Match m in Regex.Matches(curr, output_pattern, options))
                {
                    int i = 0;
                    foreach (var g in m.Groups)
                    {
                        if (i != 0)
                        {
                            Output tempOutput;
                            if (OutputDictionary.TryGetValue(g.ToString(), out tempOutput))
                            {
                                output = tempOutput;
                            }
                        }
                        i++;
                    }
                }

                foreach (Match m in Regex.Matches(curr, dimension_pattern, options))
                {
                    int i = 0;
                    foreach (var g in m.Groups)
                    {
                        if (i != 0)
                        {
                            int tempDimensions;
                            if (int.TryParse(g.ToString(), out tempDimensions))
                            {
                                dimensions = tempDimensions;
                            }
                        }
                        i++;
                    }
                }

                foreach (Match m in Regex.Matches(curr, options_pattern, options))
                {
                    int i = 0;
                    foreach (var g in m.Groups)
                    {
                        if (i != 0)
                        {
                            string[] tempOptions = g.ToString().Split(',');
                            foreach (var currOption in tempOptions)
                            {
                                enabledOptions.Add(currOption);
                            }
                        }
                        i++;
                    }
                }

                foreach (Match m in Regex.Matches(curr, fileOut_pattern, options))
                {
                    int i = 0;
                    foreach (var g in m.Groups)
                    {
                        if (i != 0)
                        {
                            outputFilePath = FilePathDecode(g.ToString());
                            writeToFile = true;
                        }
                        i++;
                    }
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
                Console.WriteLine("Output redirected to: " + outputFilePath);
                outputStream = new StreamWriter(outputFilePath);
            }

            if (operation == Operation.cdf)
            {
                if (!normalDistributionParameter.HasValue)
                {
                    Console.WriteLine("What is the z-score?");
                    normalDistributionParameter = double.Parse(Console.ReadLine());
                }

                PrintLine(NormalDistribution.Cdf(normalDistributionParameter.Value), writeToFile, outputStream);


                return;
            }

            if (operation == Operation.invCdf)
            {
                if (!normalDistributionParameter.HasValue)
                {
                    Console.WriteLine("What is the probability?");
                    normalDistributionParameter = double.Parse(Console.ReadLine());
                }

                PrintLine(NormalDistribution.InvCdf(normalDistributionParameter.Value), writeToFile, outputStream);

                return;
            }

            if (setRaw.Length == 0)
            {
                Console.WriteLine("\nType your set here:");
                setRaw.Append(Console.ReadLine());
                Console.WriteLine("\nYour set:");
                Console.WriteLine(setRaw);
                Console.WriteLine();
            }

            List<string> setStringList = setRaw.Replace("(", "").Replace(")", "").ToString().Split(',').ToList();
            if (dimensions == 1)
            {
                DataSet set = new DataSet(setStringList.Select(s => double.Parse(s)).ToList());
                switch (operation)
                {
                    case Operation.list:
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
                                {
                                    Console.WriteLine("Is your set a population? (Y/N)\n\nIf you don't know, select \"No\"");
                                    isPopulation = Console.ReadLine().ToUpper() == "Y";
                                }
                            }

                            if (isPopulation.Value)
                            {
                                Print(set.StandardizeSet(true).List(output), writeToFile, outputStream);
                            }
                            else
                            {
                                Print(set.StandardizeSet(false).List(output), writeToFile, outputStream);
                            }

                        }
                        break;
                }
            }
            else
            {
                VectorSet vectorSet;

                List<double>[] dimensionSets = new List<double>[dimensions];
                DataSet[] dimensionDataSet = new DataSet[dimensions];

                for (int i = 0; i < dimensions; i++)
                {
                    dimensionSets[i] = new List<double>();
                }

                for (int i = 0; i < setStringList.Count; i++)
                {
                    dimensionSets[i % dimensions].Add(double.Parse(setStringList[i]));
                }

                for (int i = 0; i < dimensions; i++)
                {
                    dimensionDataSet[i] = new DataSet(dimensionSets[i]);
                }

                vectorSet = new VectorSet(dimensionDataSet);

                switch (operation)
                {
                    case Operation.list:
                        Print(vectorSet.List(output), writeToFile, outputStream);
                        break;
                    case Operation.summary:
                        Print(vectorSet.Summarize(output), writeToFile, outputStream);
                        break;
                    case Operation.plot:
                        Plot plot = new Plot(vectorSet);
                        string filename = "";
                        if (enabledOptions.Contains("linreg"))
                        {
                            filename = await plot.Draw(RegressionLines.linear);
                            double[] coefficients = new LinearRegressionLine().Calculate(vectorSet);
                            PrintLine($"\n" +
                                $"\ty=a+bx" +
                                $"\n\n" +
                                $"\ta={coefficients[0]}\n" +
                                $"\tb={coefficients[1]}" +
                                $"\n",
                                writeToFile, outputStream);
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
                                {
                                    Console.WriteLine("Is your set a population? (Y/N)\n\nIf you don't know, select \"No\"");
                                    isPopulation = Console.ReadLine().ToUpper() == "Y";
                                }
                            }

                            if (isPopulation.Value)
                            {
                                Print(vectorSet.StandardizeSet(true).List(output), writeToFile, outputStream);
                            }
                            else
                            {
                                Print(vectorSet.StandardizeSet(false).List(output), writeToFile, outputStream);
                            }
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
                            Print(vectorSet.Correlation(true), writeToFile, outputStream);
                        }
                        else if (enabledOptions.Contains("sample"))
                        {
                            Print(vectorSet.Correlation(false), writeToFile, outputStream);
                        }
                        else
                        {
                            Console.WriteLine("Is your set a population? (Y/N)\n\nIf you don't know, select \"No\"");
                            bool population = Console.ReadLine().ToUpper() == "Y";
                            Print(vectorSet.Correlation(population), writeToFile, outputStream);
                        }
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
                outputStream.Close();
                outputStream.Dispose();
            }
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

        public static void PrintLine<T>(T toPrint, bool printToFileToo, StreamWriter outputStream)
        {
            Print(toPrint + "\n", printToFileToo, outputStream);
        }
    }
}
