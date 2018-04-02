using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using PxtlCa.XmlCommentMarkDownGenerator.MSBuild.Options;
using System.Text.RegularExpressions;

namespace PxtlCa.XmlCommentMarkDownGenerator.MSBuild
{
    /// <summary>
    /// A task that generates and optionally merges markdown
    /// </summary>
    public class GenerateMarkdown : Task
    {
        /// <summary>
        /// The file(s) from which to generate markdown.  This should be in XmlDocumentation format.
        /// </summary>
        [Required]
        public ITaskItem[] InputXml { get; set; }

        /// <summary>
        /// DocumentationPath is the top level directory in which 
        /// generated markdown files are created.
        /// </summary>
        [Required]
        public ITaskItem TargetDocumentDirPath { get; set; }

        /// <summary>
        /// SourceDocumentDirPath is the top level directory in which to search for markdown files.
        /// </summary>
        [Required]
        public ITaskItem SourceDocumentDirPath { get; set; }

        /// <summary>
        /// Defaults to Error. Event to execute when an unexpected tag is found. 
        /// </summary>
        public UnexpectedTagActionEnum UnexpectedTagAction { get; set; } = UnexpectedTagActionEnum.Error;

        /// <summary>
        /// Runs the task as configured
        /// </summary>
        /// <returns>true if task has succeeded</returns>
        public override bool Execute()
        {
            if (InputXml.Length == 0)
            {
                Log.LogError($"{nameof(InputXml)} cannot be empty.");
                return false;
            }

            if (File.Exists(TargetDocumentDirPath.ItemSpec))
            {
                Log.LogError($"{nameof(TargetDocumentDirPath)} must be a directory, not a file.");
                return false;
            }

            if (File.Exists(SourceDocumentDirPath.ItemSpec))
            {
                Log.LogError($"{nameof(SourceDocumentDirPath)} must be a directory, not a file.");
                return false;
            }

            if (!Directory.Exists(SourceDocumentDirPath.ItemSpec))
            {
                Log.LogError($"{nameof(SourceDocumentDirPath)} directory does not exist.");
                return false;
            }

            var markdownSourceDocuments = Directory.EnumerateFiles(SourceDocumentDirPath.ItemSpec, "*.md")
                .Select(f => GetSourceDocument(f))
                .ToList();

            //try for several sources of header data
            IEnumerable<TransformationInput> sourceDocumentsToExecute = null;
            //check for YAML-header files
            var sourceDocsWithYaml = markdownSourceDocuments.Where(doc =>
                doc.Options != null
            );
            if (sourceDocsWithYaml.Count() > 0)
            {
                sourceDocumentsToExecute = sourceDocsWithYaml.Select(src
                    => new TransformationInput() { SourceDocument = src, InputXml = InputXml }
                );
            }

            if (sourceDocumentsToExecute == null)
            {
                //try check Readme file
                var readmeSourceDocument = markdownSourceDocuments.SingleOrDefault(src =>
                    Path.GetFileNameWithoutExtension(src.FileName)
                    .Equals("Readme", StringComparison.OrdinalIgnoreCase)
                );
                if (readmeSourceDocument != null)
                {
                    sourceDocumentsToExecute = new[] {
                        new TransformationInput() { SourceDocument = readmeSourceDocument, InputXml = InputXml }
                    };
                }
            }

            if (sourceDocumentsToExecute == null)
            {
                //try check for filename match
                var matchedNameInputs = markdownSourceDocuments.Join(
                    InputXml,
                    src => Path.GetFileNameWithoutExtension(src.FileName),
                    xml => Path.GetFileNameWithoutExtension(xml.ItemSpec),
                    (src, xml) => new TransformationInput() { SourceDocument = src, InputXml = new[] { xml } }
                );
                if (matchedNameInputs.Count() > 0)
                {
                    sourceDocumentsToExecute = matchedNameInputs;
                }
            }

            if (sourceDocumentsToExecute == null)
            {
                //try create dummies source file, use matching name to xml file
                sourceDocumentsToExecute = InputXml.Select(xml => new TransformationInput()
                {
                    InputXml = new[] { xml },
                    SourceDocument = new SourceDocument()
                    {
                        Body = string.Empty,
                        Options = null,
                        FileName = Path.GetFileNameWithoutExtension(xml.ItemSpec)
                    }
                });
            }

            try
            {
                GenerateFiles(sourceDocumentsToExecute);
                return true;
            }
            catch (Exception ex)
            {
                LoggedException = ex;
                Log.LogErrorFromException(ex);
            }
            return false;
        }

        /// <summary>
        /// Load a Markdown file and parse out its Yaml header if any, building a SourceDocument object
        /// </summary>
        /// <param name="fileName">A Markdown file with a PanDoc-style YAML header.</param>
        /// <returns>A SourceDocument object. Options is null </returns>
        public SourceDocument GetSourceDocument(string fileName)
        {
            GetFileSections(fileName, out string frontMatter, out string body);
            return new SourceDocument()
            {
                Options = ReadOptionsFromString(frontMatter),
                FileName = fileName,
                Body = body
            };
        }

        /// <summary>
        /// Use this to handle front matter in markdown files
        /// </summary>
        /// <param name="filePath">the path to the file</param>
        /// <param name="frontMatter">the front matter found.  Empty string if missing or trivial.</param>
        /// <param name="body">the body of the source document</param>
        /// <returns>true if front matter indicator(s) are found</returns>
        public static void GetFileSections(string filePath, out string frontMatter, out string body)
        {
            var lines = File.ReadLines(filePath);
            var firstLine = lines.FirstOrDefault() ?? string.Empty;
            var yamlLinesCount = 0;

            if (firstLine.StartsWith("---")) //yaml start
            {
                var yamlSB = new StringBuilder();
                yamlLinesCount = 1;
                foreach (var line in lines)
                {
                    if (line.StartsWith("---") && yamlLinesCount > 1)
                    {
                        break;
                    }
                    yamlSB.AppendLine(line);
                    yamlLinesCount += 1;
                }
                
                frontMatter = yamlSB.ToString();
            }
            else
            {
                frontMatter = string.Empty;
            }
            body = String.Join(Environment.NewLine, lines.Skip(yamlLinesCount));
        }

        private static readonly Regex EmptyFrontMatterRegex
            = new Regex(@"---\s+");
        private YamlOptions ReadOptionsFromString(string frontMatter)
        {
            //HACK: Yaml deserializer continues a simple ---\r\n\ to *not* be a valid YAML doc.
            //we want to consider that a valid YAML doc.
            if(EmptyFrontMatterRegex.IsMatch(frontMatter))
            {
                return new YamlOptions();
            }

            var input = new StringReader(frontMatter);
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
                .IgnoreUnmatchedProperties()
                .Build();
            var options = deserializer.Deserialize<YamlOptions>(input);
            return options;
        }

        /// <summary>
        /// for testing.  
        /// sets the exception for throw outside the catch
        /// </summary>
        public Exception LoggedException { get; set; }

        private void GenerateFiles(IEnumerable<TransformationInput> inputs)
        {
            var groupedInputs = inputs.GroupBy(input => input.RelativeOutputPath, StringComparer.OrdinalIgnoreCase);

            foreach (var inputGroup in groupedInputs)
            {
                var relativeOutputPath = inputGroup.Key;
                var fullOutputPath = Path.Combine(TargetDocumentDirPath.ItemSpec, relativeOutputPath);
                Directory.CreateDirectory(Path.GetDirectoryName(fullOutputPath));

                var inputsForOutput = inputGroup.OrderBy(i => i.MergeSequence, CrudeNaturalSort.Instance);
                using (var sw = new StreamWriter(fullOutputPath))
                {
                    foreach (var input in inputsForOutput)
                    {
                        sw.WriteLine(input.SourceDocument.Body);
                        foreach (var inputXml in input.InputXml)
                        {
                            var context = new ConversionContext() {
                                AssemblyName = Path.GetFileNameWithoutExtension(inputXml.ItemSpec),
                                UnexpectedTagAction = UnexpectedTagAction,
                                WarningLogger = new BuildMessageWarningLogger(Log)
                            };

                            using (var sr = new StreamReader(inputXml.ItemSpec))
                            {
                                var xml = sr.ReadToEnd();
                                var doc = XDocument.Parse(xml);
                                var md = doc.Root.ToMarkDown(context);
                                sw.WriteLine(md);
                            }
                        }                      
                    }
                }
            }
        }
    }
}
