using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Build.Framework;
using Microsoft.Build.Tasks;
using Microsoft.Build.Utilities;
using PxtlCa.XmlCommentMarkdownGenerator;
using PxtlCa.XmlCommentMarkDownGenerator.MSBuild;
using Rhino.Mocks;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml;
using System.Linq;

namespace PxtlCa.XmlCommentMarkdownGenerator.MSBuild.Test
{
    [TestClass]
    public class BuildEngine
    {
        [TestMethod]
        [DeploymentItem("Docs")]
        public void ExecuteMerge()
        {
            PrepareDocsDirectory();
            var mockRepo = new MockRepository();
            var buildEngine = mockRepo.Stub<IBuildEngine>();

            var inputPath = @"..\..\..\PxtlCa.XmlCommentMarkdownGenerator.MSBuild\bin\Debug\PxtlCa.XmlCommentMarkDownGenerator.MSBuild.xml";
            var docPath = @"Docs";
            var outputFile = new TaskItem(@"..\..\Readme.md");

            
            var inputXml = new ITaskItem[] { new TaskItem(inputPath) };
            var documentPath = new TaskItem(docPath);
            var merge = true;

            var task = new GenerateMarkdown
            {
                BuildEngine = buildEngine,
                DocumentationPath = documentPath,
                InputXml = inputXml,
                MergeFiles = merge,
                OutputFile = outputFile
            };

            task.Execute();

            var expectFileExists = true;
            var fileActuallyExists = System.IO.File.Exists(outputFile.ItemSpec);

            Assert.AreEqual(expectFileExists, fileActuallyExists);
        }

        [TestMethod]
        [DeploymentItem("Docs")]
        public void TestAvertMerge()
        {
            PrepareDocsDirectory(overrideMerge:true);
            var mockRepo = new MockRepository();
            var buildEngine = mockRepo.Stub<IBuildEngine>();

            var inputPath = @"..\..\..\PxtlCa.XmlCommentMarkdownGenerator.MSBuild\bin\Debug\PxtlCa.XmlCommentMarkDownGenerator.MSBuild.xml";
            var docPath = @"Docs";
            var outputFile = new TaskItem(@"..\..\Readme.md");


            var inputXml = new ITaskItem[] { new TaskItem(inputPath) };
            var documentPath = new TaskItem(docPath);
            var merge = true;

            var task = new GenerateMarkdown
            {
                BuildEngine = buildEngine,
                DocumentationPath = documentPath,
                InputXml = inputXml,
                MergeFiles = merge,
                OutputFile = outputFile
            };

            task.Execute();

            var expectFileExists = true;
            var fileActuallyExists = System.IO.File.Exists(outputFile.ItemSpec);

            Assert.AreEqual(expectFileExists, fileActuallyExists);

            var docCount = Directory.EnumerateFiles(@"Docs", "*.md", SearchOption.TopDirectoryOnly).ToList().Count;

            //different than the case where the files are merged (into _one_ file)
            var expectedDocCount = 2;

            Assert.AreEqual(expectedDocCount, docCount);
        }

        [TestMethod]
        [DeploymentItem("Docs")]
        public void HandleUnexpectedTag()
        {
            PrepareDocsDirectory();
            var mockRepo = new MockRepository();
            var buildEngine = mockRepo.Stub<IBuildEngine>();

            var inputPath = @"..\..\..\PxtlCa.XmlCommentMarkdownGenerator.MSBuild\bin\Debug\PxtlCa.XmlCommentMarkDownGenerator.MSBuild.xml";
            var toAlter = File.ReadAllText(inputPath);
            toAlter = toAlter.Replace(@"<returns>", @"<returns><inheritdoc>X</inheritdoc>");

            var alteredPath = inputPath.Replace("MSBuild.xml", "MSBuild_alteredHandled.xml");
            File.WriteAllText(alteredPath, toAlter);

            var docPath = @"Docs";
            var outputFile = new TaskItem(@"..\..\Readme.md");


            var inputXml = new ITaskItem[] { new TaskItem(alteredPath) };

            var documentPath = new TaskItem(docPath);
            var merge = true;

            var task = new GenerateMarkdown
            {
                BuildEngine = buildEngine,
                DocumentationPath = documentPath,
                InputXml = inputXml,
                MergeFiles = merge,
                UnexpectedTagAction = XmlCommentMarkDownGenerator.UnexpectedTagActionEnum.Accept,
                OutputFile = outputFile
            };

            task.Execute();

            if (null != task.LoggedException)
            {
                throw task.LoggedException;
            }

            var expectFileExists = true;
            var fileActuallyExists = System.IO.File.Exists(outputFile.ItemSpec);

            Assert.AreEqual(expectFileExists, fileActuallyExists);
        }


        [TestMethod]
        [DeploymentItem("Docs")]
        [ExpectedException(typeof(XmlException))]
        public void ExpectedErrorUnexpectedTag()
        {
            PrepareDocsDirectory();
            var mockRepo = new MockRepository();
            var buildEngine = mockRepo.Stub<IBuildEngine>();

            var inputPath = @"..\..\..\PxtlCa.XmlCommentMarkdownGenerator.MSBuild\bin\Debug\PxtlCa.XmlCommentMarkDownGenerator.MSBuild.xml";
            var toAlter = File.ReadAllText(inputPath);
            toAlter = toAlter.Replace(@"<returns>", @"<returns><inheritdoc>X</inheritdoc>");

            var alteredPath = inputPath.Replace("MSBuild.xml", "MSBuild_alteredError.xml");
            File.WriteAllText(alteredPath, toAlter);

            var docPath = @"Docs";
            var outputFile = new TaskItem(@"..\..\Readme.md");


            var inputXml = new ITaskItem[] { new TaskItem(alteredPath) };

            var documentPath = new TaskItem(docPath);
            var merge = true;

            var task = new GenerateMarkdown
            {
                BuildEngine = buildEngine,
                DocumentationPath = documentPath,
                InputXml = inputXml,
                MergeFiles = merge,
                OutputFile = outputFile,
                UnexpectedTagAction = XmlCommentMarkDownGenerator.UnexpectedTagActionEnum.Error
            };

            task.Execute();

            if(null != task.LoggedException)
            {
                throw task.LoggedException;
            }
            var expectFileExists = true;
            var fileActuallyExists = System.IO.File.Exists(outputFile.ItemSpec);

            Assert.AreEqual(expectFileExists, fileActuallyExists);
        }

        public static void PrepareDocsDirectory(bool overrideMerge = false)
        {
            var mergeDirectiveInserted = false;
            if(Directory.Exists("Docs"))
            {
                Directory.Delete("Docs", true);
            }
            Directory.CreateDirectory("Docs");
            var filesToMove = Directory.EnumerateFiles(Environment.CurrentDirectory, "*.md", SearchOption.TopDirectoryOnly).ToList();
            foreach (var mdFile in filesToMove)
            {
                if(overrideMerge && (!mergeDirectiveInserted))
                {
                    mergeDirectiveInserted = TryUpdateFile(mergeDirectiveInserted, mdFile);
                }
                File.Copy(mdFile, $@".\Docs\{Path.GetFileName(mdFile)}");
            }
        }

        private static bool TryUpdateFile(bool mergeDirectiveInserted, string mdFile)
        {
            if (GenerateMarkdown.TryGetFrontMatter(mdFile, out string frontMatter, out bool isEmpty))
            {
                if (isEmpty)
                {
                    //get all the lines starting with the second "---" line
                    var lines = File.ReadLines(mdFile).Skip(1).ToList();
                    //now insert the front matter in reverse order
                    lines.Insert(0, "AllowedCustomTags: all");
                    lines.Insert(0, "MergeXmlComments: false");
                    lines.Insert(0, "---");
                    //overwrite the file with the update
                    File.WriteAllText(mdFile, string.Join(Environment.NewLine, lines));
                    //stop any additional preprocesssing
                    mergeDirectiveInserted = true;
                }
            }

            return mergeDirectiveInserted;
        }
    }
}
