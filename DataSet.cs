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

            return $"\t{min}\t{Q1}\t{med}\t{Q3}\t{max}";
        }

        public double Mean() {
            Console.WriteLine(set[0]);
            Console.WriteLine(set[1]);
            double total = 0;
            foreach (double curr in set) {
                total += curr;
            }

            return total / set.Count;
        }
    }
}
