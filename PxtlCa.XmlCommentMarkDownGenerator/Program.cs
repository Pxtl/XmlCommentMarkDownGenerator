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
                // for in case the target directory does not already exist, create it now
                if (!System.IO.Directory.Exists(Path.GetDirectoryName(options.OutputFile)))
                {
                    System.IO.Directory.CreateDirectory(Path.GetDirectoryName(options.OutputFile));
                }
                // consume Options instance properties
                var inReader = options.ConsoleIn
                    ? Console.In
                    : new StreamReader(options.InputFile);
                using (var outWriter = options.ConsoleIn
                    ? Console.Out
                    : new StreamWriter(options.OutputFile)
                    )
                {
                    var xml = inReader.ReadToEnd();
                    var doc = XDocument.Parse(xml);
                    var context = new ConversionContext() { UnexpectedTagAction = options.UnexpectedTagAction, WarningLogger = new TextWriterWarningLogger(Console.Error) };
                    var md = doc.Root.ToMarkDown(context);
                    outWriter.Write(md);
                    outWriter.Close();
                }
            }
        }
    }
}
