using System;
using System.Collections.Generic;
using System.Text;

namespace Where1.stat
{
    class DataSet
    {
        private List<Double> set = new List<double>();

        public DataSet(List<double> setList)
        {
            set = setList;
            set.Sort();
        }

        public string List()
        {
            StringBuilder output = new StringBuilder();
            foreach (double curr in set)
            {
                output.Append($"\t{curr:f9}\n");
            }

            return output.ToString();
        }

        public string Summarize()
        {
            int QSize = (int)Math.Floor(set.Count / 4.0);
            double Q1 = set[QSize];
            double Q3 = set[set.Count - QSize - 1];
            double min = set[0];
            double max = set[set.Count - 1];
            double med = new DataSet(new List<Double>() { set[(int)Math.Floor((set.Count + 1) / 2.0) - 1], set[(int)Math.Ceiling((set.Count + 1) / 2.0) - 1] }).Mean();

            return $"\tMin\t\tQ1\t\tMed\t\tQ3\t\tMax" +
                    $"\n\t{min:f9}\t{Q1:f9}\t{med:f9}\t{Q3:f9}\t{max:f9}";
        }

        public double Mean() {
            double total = 0;
            foreach (double curr in set) {
                total += curr;
            }

            return total / set.Count;
        }
    }
}
