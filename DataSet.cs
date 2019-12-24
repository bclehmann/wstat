using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Where1.wstat
{
	class DataSet
	{
		private readonly List<double> set = new List<double>();

		public DataSet(List<double> setList)
		{
			set = setList;
		}

		public List<double> GetSet()
		{
			return set;
		}

		public double Get(int index)
		{
			return set[index];
		}

		public string List(Output outputFormat = Output.text)
		{
			var tempSet = set;
			tempSet.Sort();
			switch (outputFormat)
			{
				case Output.text:
					StringBuilder outputText = new StringBuilder();
					foreach (double curr in tempSet)
					{
						outputText.Append($"\t{curr}\n");
					}
					return outputText.ToString();
					break;
				case Output.json:
					return JsonSerializer.Serialize(tempSet);
					break;
				case Output.csv:
					StringBuilder outputCsv = new StringBuilder();
					foreach (double curr in tempSet)
					{
						outputCsv.Append($"{curr},");
					}
					return outputCsv.Remove(outputCsv.Length - 1, 1).ToString();
					break;
				default:
					throw new NotSupportedException("You attempted to output to a format that is not currently supported");
					break;
			}
		}

		public string Summarize(Output outputFormat = Output.text)
		{

			switch (outputFormat)
			{
				case Output.text:
					const int cellWidth = -18;//The sign determines left or right padding
					return $"\tThese are all rounded values. If you need more precision, use JSON output\n\n" +
						 $"\t{"Min",cellWidth}{"Q1",cellWidth}{"Med",cellWidth}{"Q3",cellWidth}{"Max",cellWidth}" +
						 $"\n\t{Min,cellWidth:f12}{Q1,cellWidth:f12}{Med,cellWidth:f12}{Q3,cellWidth:f12}{Max,cellWidth:f12}\n\n" +
						 $"\t{"N (Set Size)",cellWidth}{"Mean",cellWidth}{"Std. Dev. (s)",cellWidth}{"Std. Dev. (σ)",cellWidth}\n" +
						 $"\t{this.Length,cellWidth}{this.Mean,cellWidth:f12}{this.SampleStandardDeviation,cellWidth:f12}{this.PopulationStandardDeviation,cellWidth:f12}" +
						 $"\n\n" +
						 $"\tPossible Outliers:\n" +
						 $"{new DataSet(this.PossibleOutliers).List()}";
					break;
				case Output.json:
					return JsonSerializer.Serialize(this);
					break;
				default:
					throw new NotSupportedException("You attempted to output to a format that is not currently supported");
					break;
			}

		}

		public DataSet StandardizeSet(bool population)
		{ //z-scores
			double stdDev = population ? this.PopulationStandardDeviation : this.SampleStandardDeviation;
			double mean = this.Mean;

			List<double> tempSet = new List<double>(set.Count);
			foreach (var curr in set)
			{
				tempSet.Add((curr - mean) / stdDev);
			}

			return new DataSet(tempSet);
		}

		public DataSet Quantile(int n)
		{
			double size = (set.Count / (double)n);//The cast to double is required to choose a suitable overload

			List<double> outputList = new List<double>();

			var tempSet = set;
			tempSet.Sort();
			for (int i = 1; i < set.Count; i++)
			{
				if (i == 0) {
					continue;
				}

				int x = (int)Math.Floor(i * size);

				if (x < tempSet.Count)
				{
					outputList.Add(tempSet[x]);
				}
				else
				{
					break;
				}
			}

			return new DataSet(outputList);
		}

		public List<double> PossibleOutliers
		{
			get
			{
				return set.FindAll(m => m < Q1 - 1.5 * (Q3 - Q1) || m > Q3 + 1.5 * (Q3 - Q1));
			}
		}


		private int QSize { get { return (int)Math.Floor(set.Count / 4.0); } }
		public double Q1
		{
			get
			{
				var tempSet = set;
				tempSet.Sort();
				return tempSet[QSize];
			}
		}
		public double Q3 { get { return set[set.Count - QSize - 1]; } }
		public double Min
		{
			get
			{
				var tempSet = set;
				tempSet.Sort();
				return tempSet[0];
			}
		}
		public double Max
		{
			get
			{
				var tempSet = set;
				tempSet.Sort();
				return tempSet[set.Count - 1];
			}
		}
		public double Med
		{
			get
			{
				var tempSet = set;
				tempSet.Sort();
				return new DataSet(new List<Double>() { tempSet[(int)Math.Floor((set.Count + 1) / 2.0) - 1], tempSet[(int)Math.Ceiling((set.Count + 1) / 2.0) - 1] }).Mean;
			}
		}
		public int Length { get { return set.Count; } }

		public double Mean
		{
			get
			{
				double total = 0;
				foreach (double curr in set)
				{
					total += curr;
				}

				return total / set.Count;
			}
		}


		public double PopulationStandardDeviation
		{
			get
			{
				double sumSquaredDifference = 0;
				double mean = this.Mean;

				foreach (double curr in set)
				{
					sumSquaredDifference += Math.Pow((curr - mean), 2);
				}

				return (Math.Sqrt(sumSquaredDifference / set.Count));
			}

		}

		public double SampleStandardDeviation
		{
			get
			{
				double sumSquaredDifference = 0;
				double mean = this.Mean;

				foreach (double curr in set)
				{
					sumSquaredDifference += Math.Pow((curr - mean), 2);
				}

				return (Math.Sqrt(sumSquaredDifference / (set.Count - 1)));
			}
		}

	}
}
