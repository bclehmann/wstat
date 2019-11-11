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
                        outputText.Append($"\t{curr:f9}\n");
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
                        outputCsv.Append($"{curr:f9},");
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
                    return $"\tMin\t\tQ1\t\tMed\t\tQ3\t\tMax" +
                         $"\n\t{Min:f9}\t{Q1:f9}\t{Med:f9}\t{Q3:f9}\t{Max:f9}\n\n" +
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

        public DataSet StandardizeSet(bool population) { //z-scores
            double stdDev = population ? this.PopulationStandardDeviation : this.SampleStandardDeviation;
            double mean = this.Mean;

            List<double> tempSet = new List<double>(set.Count);
            foreach (var curr in set) {
                tempSet.Add((curr - mean) / stdDev);
            }

            return new DataSet(tempSet);
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
