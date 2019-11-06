using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Where1.stat
{
    class VectorSet
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
                        output.Append($"\nDimension {i}:\n\n{DataSets[i].Summarize(Output.text)}\n\n\n");
                    }
                    return output.ToString();
                case Output.json:
                    return JsonSerializer.Serialize(DataSets);
                default:
                    throw new NotSupportedException("You attempted to output to a format that is not currently supported");
                    break;
            }

        }

        public double[] LeastSquareResidualRegressionLine()
        {
            if (Dimensions != 2)
            {
                throw new NotSupportedException("linreg only supported in two dimensions currently");
            }

            double a = 1;
            double b;

            double sumXYResidual = 0;
            double sumXSquareResidual = 0;


            //double aIncrement = 1;

            for (int i = 0; i < Length; i++)
            {
                sumXYResidual += (Vectors[i][0] - DataSets[0].Mean) * (Vectors[i][1] - DataSets[1].Mean);
                sumXSquareResidual += Math.Pow((Vectors[i][0] - DataSets[0].Mean), 2);
            }

            b = sumXYResidual / (sumXSquareResidual);

            a = DataSets[1].Mean - (b * DataSets[0].Mean);//LSRL always passes through the point (x̅,y̅)

            //while (aIncrement > 2 >> 10)
            //{
            //    double initial = LeastSquareRegression(a, b);
            //    bool changed = false;

            //    if (LeastSquareRegression(a - aIncrement, b) < initial)
            //    {
            //        a -= aIncrement;
            //        changed = true;
            //    }
            //    else if (LeastSquareRegression(a + aIncrement, b) < initial)
            //    {
            //        a += aIncrement;
            //        changed = true;
            //    }

            //    if (!changed)
            //    {

            //        aIncrement /= 2;
            //    }
            //}

            return new double[] { a, b };
        }

        private double LeastSquareRegression(double a, double b)
        {
            if (Dimensions != 2)
            {
                throw new NotSupportedException("linreg only supported in two dimensions currently");
            }

            double total = 0;
            foreach (var curr in Vectors)
            {
                total += Math.Pow(curr[1] - (a + (b * curr[0])), 2);
            }

            return total;
        }

    }
}
