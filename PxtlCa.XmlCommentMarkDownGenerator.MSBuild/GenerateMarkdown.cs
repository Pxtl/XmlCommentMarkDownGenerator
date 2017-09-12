using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.IO;
using System.Xml.Linq;

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
        /// DocumentationPath is the top level directory in which to search for files.
        /// It is also the path where generated markdown files are created.
        /// </summary>
        [Required]
        public ITaskItem DocumentationPath { get; set; }

        /// <summary>
        /// Whether the generated markdown files should merge.  Only valid if multiple markdown files exist.
        /// DocumentationPath is the top level directory in which to search for files.
        /// Both existing markdown files and the generated files are merged.
        /// </summary>
        [Required]
        public bool MergeFiles { get; set; }

        /// <summary>
        /// The file to be created by the merge.  Unused if MergeFiles evaluates to false.
        /// </summary>
        public ITaskItem OutputFile { get; set; }



        /// <summary>
        /// Runs the task as configured
        /// </summary>
        /// <returns>true if task has succeeded</returns>
        public override bool Execute()
        {
            if(InputXml.Length == 0)
            {
                Log.LogError("InputXml cannot be empty");
            }
            else if(MergeFiles && (null == OutputFile))
            {
                Log.LogError("OutputFile must be specified if input files are merged");
            }
            else if(DocumentationPathIsFile && InputXml.Length != 1)
            {
                Log.LogError("DocumentationPath must specify a directory is more than one input XML value is supplied");
            }
            else
            {
                try
                {
                    CreateDirectoryIfNeeded();
                    GenerateFiles();
                    if(MergeFiles)
                    {
                        Merge();
                    }
                    return true;
                }
                catch(Exception ex)
                {
                    Log.LogErrorFromException(ex);
                }
            }
            return false;
        }

        private void Merge()
        {
            //get all md files in Documentation Path
            //except those generated in this task
            var otherMDFiles = Directory.EnumerateFiles(DocumentationPath.ItemSpec, "*.md", SearchOption.AllDirectories).ToList();
            otherMDFiles = otherMDFiles.Except(GeneratedMDFiles).ToList();
            var mergeInto = otherMDFiles.FirstOrDefault();
            if (null == mergeInto)
            {
                mergeInto = GeneratedMDFiles.First();
                File.Copy(mergeInto, OutputFile.ItemSpec, true);
                foreach (var mdFile in GeneratedMDFiles.Skip(1))
                {
                    File.AppendAllText(OutputFile.ItemSpec, Environment.NewLine);
                    File.AppendAllText(OutputFile.ItemSpec, File.ReadAllText(mdFile));
                }
            }
            else
            {
                File.Copy(mergeInto, OutputFile.ItemSpec, true);
                foreach (var mdFile in otherMDFiles.Skip(1))
                {
                    File.AppendAllText(OutputFile.ItemSpec, Environment.NewLine);
                    File.AppendAllText(OutputFile.ItemSpec, File.ReadAllText(mdFile));
                }
                foreach (var mdFile in GeneratedMDFiles)
                {
                    File.AppendAllText(OutputFile.ItemSpec, Environment.NewLine);
                    File.AppendAllText(OutputFile.ItemSpec, File.ReadAllText(mdFile));
                }
            }
        }

        private void GenerateFiles()
        {
            foreach (var inputFile in InputXml)
            {
                var mdOutput = OutputPath(inputFile.ItemSpec);
                GeneratedMDFiles.Add(mdOutput);
                var sr = new StreamReader(inputFile.ItemSpec);
                using (var sw = new StreamWriter(mdOutput))
                {
                    var xml = sr.ReadToEnd();
                    var doc = XDocument.Parse(xml);
                    var md = doc.Root.ToMarkDown();
                    sw.Write(md);
                    sw.Close();
                }
            }
        }

        private string OutputPath(string inputXml)
        {
            if (DocumentationPathIsFile)
            {
                return DocumentationPath.ItemSpec;
            }
            return $@"{DocumentationPath.ItemSpec}\{Path.GetFileNameWithoutExtension(inputXml)}.md";
        }

        private bool DocumentationPathIsFile
        {
            get { return File.Exists(DocumentationPath.ItemSpec); }
        }

        /// <summary>
        /// The files generated during execution of the task
        /// </summary>
        public List<string> GeneratedMDFiles { get; private set; } = new List<string>();

        private void CreateDirectoryIfNeeded()
        {
            if ((!DocumentationPathIsFile) && (!Directory.Exists(DocumentationPath.ItemSpec)))
            {
                Directory.CreateDirectory(DocumentationPath.ItemSpec);
            }
        }
    }
}
