using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PxtlCa.XmlCommentMarkDownGenerator
{
    class Options
    {
        [Option('i', "inputfile", Required = true, MutuallyExclusiveSet = "Input", HelpText = "Input xml file to read.")]
        public string InputFile { get; set; }

        [Option("cin", Required = true, MutuallyExclusiveSet = "Input", HelpText = "Read input from console instead of file.")]
        public bool ConsoleIn { get; set; }

        [Option('o', "outputfile", Required = true, MutuallyExclusiveSet = "Output", HelpText = "Output md file to write.")]
        public string OutputFile { get; set; }

        [Option("cout", Required = true, MutuallyExclusiveSet = "Output", HelpText = "Write output to console instead of file.")]
        public bool ConsoleOut { get; set; }

        [Option('v', null, HelpText = "Print details during execution.")]
        public bool Verbose { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            // this without using CommandLine.Text
            //  or using HelpText.AutoBuild
            var usage = new StringBuilder();
            usage.AppendLine("Quickstart Application 1.0");
            usage.AppendLine("Read user manual for usage instructions...");
            return usage.ToString();
        }
    }
}
