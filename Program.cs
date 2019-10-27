using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Where1.stat.Graph;

namespace Where1.stat
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
            { "plot", "operation=plot" }
        };

        private static Dictionary<string, Operation> OperationDictionary = new Dictionary<string, Operation>() {
            { "summary", Operation.summary },
            { "list", Operation.list },
            { "plot", Operation.plot }
        };

        private static Dictionary<string, Output> OutputDictionary = new Dictionary<string, Output>() {
            { "text", Output.text },
            { "json", Output.json },
            { "csv", Output.csv },
        };

        static void Main(string[] args)
        {
            const string set_pattern = @"set=(.+)";
            const string operation_pattern = @"operation=(.+)";
            const string output_pattern = @"output=(.+)";
            const string dimension_pattern = @"dimensions=(\d+)";
            RegexOptions options = RegexOptions.Multiline;

            StringBuilder setRaw = new StringBuilder();
            Operation operation = Operation.list;
            Output output = Output.text;
            int dimensions = 1;


            string[] expandedArgs = new string[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                if (Shortcuts.ContainsKey(args[i].ToLower()))
                {
                    expandedArgs[i] = Shortcuts.GetValueOrDefault(args[i].ToLower());
                }
                else
                {
                    expandedArgs[i] = args[i];
                }
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
            }

            if (setRaw.Length == 0)
            {
                setRaw.Append(Console.ReadLine());
            }

            List<string> setStringList = setRaw.Replace("(", "").Replace(")", "").ToString().Split(',').ToList();
            if (dimensions == 1)
            {
                DataSet set = new DataSet(setStringList.Select(s => double.Parse(s)).ToList());
                switch (operation)
                {
                    case Operation.list:
                        Console.Write(set.List(output));
                        break;
                    case Operation.summary:
                        Console.Write(set.Summarize(output));
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
                        Console.Write(vectorSet.List(output));
                        break;
                    case Operation.summary:
                        Console.Write(vectorSet.Summarize(output));
                        break;
                    case Operation.plot:
                        Plot plot = new Plot(vectorSet);
                        Console.WriteLine($"Filepath: {plot.Draw()}");
                        break;
                }

            }




            //foreach (var curr in set) {
            //    Console.WriteLine(curr);
            //}


        }


    }
}
