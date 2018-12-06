using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Obsoletion;

namespace ObsoletionLogger
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = args[0];

            var processor = new FileProcessor(new Parser(Path.Combine(Environment.CurrentDirectory, "legacy.csv")));
            var list = new List<Result>();
            foreach (var file in Directory.EnumerateFiles(path, "*.cs", SearchOption.AllDirectories))
            {
                list.AddRange(processor.ProcessFile(file));
            }

            foreach (var result in list)
            {
                Console.WriteLine(result);
            }

            var csv = new StringBuilder();
            csv.AppendLine(string.Join(",", "NAME", "TYPE", "RECOMMENDATION", "DATE", "MESSAGE"));
            foreach (var result in list)
            {
                csv.AppendLine(result.ToString());  
            }
            File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "obsoletions.csv"), csv.ToString());
        }
    }
}
