using CommandLine;
using CommandLine.Text;

namespace PxtlCa.XmlCommentMarkDownGenerator
{
    class Options
    {
        [Option('i', "inputfile", HelpText = "Input xml file to read.")]
        public string InputFile { get; set; }

        [Option("cin", HelpText = "Read input from console instead of file.")]
        public bool ConsoleIn { get; set; }

        [Option('o', "outputfile", HelpText = "Output md file to write.")]
        public string OutputFile { get; set; }

        [Option("cout", HelpText = "Write output to console instead of file.")]
        public bool ConsoleOut { get; set; }

        //[Option('v', null, HelpText = "Print details during execution.")]
        //public bool Verbose { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
