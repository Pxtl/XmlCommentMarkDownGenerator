using System;
using System.IO;
using System.Xml.Linq;
using CommandLine;

namespace PxtlCa.XmlCommentMarkDownGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new Options();
            if (Parser.Default.ParseArguments(args, options))
            {
                if (!options.ConsoleIn && options.InputFile == null
                    || !options.ConsoleOut && options.OutputFile == null)
                {
                    Console.WriteLine(options.GetUsage());
                    return;
                }
                // consume Options instance properties
                var inReader = options.ConsoleIn
                    ? Console.In
                    : new StreamReader(options.InputFile);
                using (var outWriter = options.ConsoleOut
                    ? Console.Out
                    : new StreamWriter(options.OutputFile)
                    )
                {
                    var xml = inReader.ReadToEnd();
                    var doc = XDocument.Parse(xml);
                    var md = doc.Root.ToMarkDown();
                    outWriter.Write(md);
                    outWriter.Close();
                }
            }
        }
    }
}
