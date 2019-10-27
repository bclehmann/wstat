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

            switch (outputFormat) {
                case Output.text:
                    StringBuilder output = new StringBuilder();

                    for(int i=0; i<DataSets.Length; i++)
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
    }
}
