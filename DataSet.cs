using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Where1.stat
{
    class DataSet
    {
        private List<Double> set = new List<double>();
        Output outputFormat;

        public DataSet(List<double> setList, Output outputType = Output.text)
        {
            set = setList;
            set.Sort();
            outputFormat = outputType;
        }

        public string List()
        {


            switch (outputFormat)
            {
                case Output.text:
                    StringBuilder outputText = new StringBuilder();
                    foreach (double curr in set)
                    {
                        outputText.Append($"\t{curr:f9}\n");
                    }
                    return outputText.ToString();
                    break;
                case Output.json:
                    return JsonSerializer.Serialize(set);
                    break;
                case Output.csv:
                    StringBuilder outputCsv = new StringBuilder();
                    foreach (double curr in set)
                    {
                        outputCsv.Append($"{curr:f9},");
                    }
                    return outputCsv.Remove(outputCsv.Length - 1, 1).ToString();
                    break;
                default:
                    throw new NotSupportedException("You attempted to output to a format that is not currently supported");
                    break;
            }
        }

        public string Summarize()
        {

            switch (outputFormat)
            {
                case Output.text:
                    return $"\tMin\t\tQ1\t\tMed\t\tQ3\t\tMax" +
                         $"\n\t{min:f9}\t{Q1:f9}\t{med:f9}\t{Q3:f9}\t{max:f9}\n\n" +
                         $"\tMean\t\tStd. Dev. (s)\tStd. Dev. (σ)\n" +
                         $"\t{this.Mean:f9}\t{this.SampleStandardDeviation:f9}\t{this.PopulationStandardDeviation:f9}";
                    break;
                case Output.json:
                    return JsonSerializer.Serialize(this);
                    break;
                default:
                    throw new NotSupportedException("You attempted to output to a format that is not currently supported");
                    break;
            }

        }


        private int QSize { get { return (int)Math.Floor(set.Count / 4.0); } }
        public double Q1 { get { return set[QSize]; } }
        public double Q3 { get { return set[set.Count - QSize - 1]; } }
        public double min { get { return set[0]; } }
        public double max { get { return set[set.Count - 1]; } }
        public double med { get { return new DataSet(new List<Double>() { set[(int)Math.Floor((set.Count + 1) / 2.0) - 1], set[(int)Math.Ceiling((set.Count + 1) / 2.0) - 1] }).Mean; } }

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
