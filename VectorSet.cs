using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using Where1.wstat.Regression;

namespace Where1.wstat
{
	public class VectorSet
	{
		public readonly DataSet[] DataSets;
		public readonly List<List<double>> Vectors;
		public readonly int Length;
		public readonly int Dimensions;

		public VectorSet(params DataSet[] sets)
		{
			DataSets = sets;
			Length = DataSets[0].Length;
			Dimensions = sets.Length;

			foreach (var curr in DataSets)
			{
				if (curr.Length != Length)
				{
					throw new DimensionMismatchException();
				}
			}

			Vectors = new List<List<double>>(Length);
			for (int i = 0; i < Length; i++)
			{
				Vectors.Add(new List<double>(Dimensions));
				for (int j = 0; j < Dimensions; j++)
				{
					Vectors[i].Add(DataSets[j].Get(i));
				}
			}
		}

		public static VectorSet CreateVectorSetFromList(List<double> unwoundSet, int dimensions)
		{
			List<double>[] dimensionSets = new List<double>[dimensions];
			DataSet[] dimensionDataSet = new DataSet[dimensions];

			for (int i = 0; i < dimensions; i++)
			{
				dimensionSets[i] = new List<double>();
			}

			for (int i = 0; i < unwoundSet.Count; i++)
			{
				dimensionSets[i % dimensions].Add(unwoundSet[i]);
			}

			for (int i = 0; i < dimensions; i++)
			{
				dimensionDataSet[i] = new DataSet(dimensionSets[i]);
			}

			return new VectorSet(dimensionDataSet);
		}

		public string List(Output outputFormat)
		{

			switch (outputFormat)
			{
				case Output.text:
					StringBuilder output = new StringBuilder();
					for (int i = 0; i < Length; i++)
					{
						output.Append("\t(");
						for (int j = 0; j < Dimensions; j++)
						{
							output.Append(Vectors[i][j]);
							if (j < Dimensions - 1)
							{
								output.Append(',');
							}
						}
						output.Append(")\n");
					}

					return output.ToString();
					break;
				case Output.json:
					return JsonSerializer.Serialize(Vectors);
					break;
				case Output.csv:
					StringBuilder outputCsv = new StringBuilder();
					for (int i = 0; i < Length; i++)
					{
						outputCsv.Append("(");
						for (int j = 0; j < Dimensions; j++)
						{
							outputCsv.Append(Vectors[i][j]);
							if (j < Dimensions - 1)
							{
								outputCsv.Append(',');
							}
						}
						outputCsv.Append("),");
					}

					return outputCsv.Remove(outputCsv.Length - 1, 1).ToString();
					break;
				default:
					throw new NotSupportedException("You attempted to output to a format that is not currently supported");
					break;
			}

		}

		public string Summarize(Output outputFormat)
		{
			switch (outputFormat)
			{
				case Output.text:
					StringBuilder output = new StringBuilder();

					for (int i = 0; i < DataSets.Length; i++)
					{
						output.Append($"\nDimension {i + 1} ({DataSets.Length} total):\n\n{DataSets[i].Summarize(Output.text)}\n\n\n");
					}
					return output.ToString();
				case Output.json:
					return JsonSerializer.Serialize(DataSets);
				default:
					throw new NotSupportedException("You attempted to output to a format that is not currently supported");
					break;
			}
		}

		public List<List<double>> Quantile(int n, Output outputFormat = Output.text)
		{
			List<List<double>> dataSetList = new List<List<double>>(Dimensions);
			for (int i = 0; i < Dimensions; i++)
			{
				dataSetList.Add(DataSets[i].Quantile(n).GetSet());
			}

			return dataSetList;
		}

		public VectorSet StandardizeSet(bool population)
		{ //z-scores

			List<DataSet> tempSets = new List<DataSet>(Dimensions);
			foreach (var curr in DataSets)
			{
				tempSets.Add(curr.StandardizeSet(population));
			}

			return new VectorSet(tempSets.ToArray());
		}

		public VectorSet ResidualSet(IRegressionLine regline)
		{
			List<double> residList = new List<double>(Length);
			foreach (var curr in Vectors)
			{
				residList.Add(regline.Residual(this, curr.ToArray()));
			}
			DataSet[] set = new DataSet[Dimensions];

			for (int i = 0; i < set.Length - 1; i++)
			{
				set[i] = DataSets[i];
			}


			set[set.Length - 1] = new DataSet(residList);
			return new VectorSet(set);

		}

		public double Correlation(bool population)
		{
			if (Dimensions != 2)
			{
				throw new NotSupportedException("This is a 2D only feature");
			}

			VectorSet zSet = this.StandardizeSet(population);

			double sumProduct = 0;
			foreach (var curr in zSet.Vectors)
			{
				double el = 1;
				foreach (var curr2 in curr)
				{
					el *= curr2;
				}
				sumProduct += el;
			}

			double coefficient = population ? 1.0 / this.Length : 1.0 / (this.Length - 1);

			return coefficient * sumProduct;
		}

	}
}
