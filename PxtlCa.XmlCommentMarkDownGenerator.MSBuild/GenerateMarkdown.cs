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
    public class GenerateMarkdown : Task
    {
        [Required]
        public ITaskItem[] InputXml { get; set; }

        [Required]
        public ITaskItem DocumentationPath { get; set; }

        [Required]
        public bool MergeFiles { get; set; }

        public ITaskItem OutputFile { get; set; }




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
            if(null == mergeInto)
            {
                mergeInto = GeneratedMDFiles.First();
                File.Copy(mergeInto, OutputFile.ItemSpec, true);
                foreach(var mdFile in GeneratedMDFiles.Skip(1))
                {
                    File.AppendAllText(OutputFile.ItemSpec, Environment.NewLine);
                    File.AppendAllText(OutputFile.ItemSpec, File.ReadAllText(mdFile));
                }
            }
            else if(otherMDFiles.Count > 1)
            {
                 File.Copy(mergeInto, OutputFile.ItemSpec, true);
                foreach (var mdFile in otherMDFiles.Skip(1))
                {
                    File.AppendAllText(OutputFile.ItemSpec, Environment.NewLine);
                    File.AppendAllText(OutputFile.ItemSpec, File.ReadAllText(mdFile));
                }
                foreach(var mdFile in GeneratedMDFiles)
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
            return $@"{DocumentationPath.ItemSpec}\{inputXml.Replace(".xml", ".md")}";
        }

        private bool DocumentationPathIsFile
        {
            get { return File.Exists(DocumentationPath.ItemSpec); }
        }

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
