using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Build.Framework;
using Microsoft.Build.Tasks;
using Microsoft.Build.Utilities;
using PxtlCa.XmlCommentMarkdownGenerator;
using PxtlCa.XmlCommentMarkDownGenerator.MSBuild;
using Rhino.Mocks;

namespace PxtlCa.XmlCommentMarkdownGenerator.MSBuild.Test
{
    [TestClass]
    public class BuildEngine
    {
        [TestMethod]
        public void ExecuteMerge()
        {
            var mockRepo = new MockRepository();
            var buildEngine = mockRepo.Stub<IBuildEngine>();

            var inputPath = @"..\..\..\PxtlCa.XmlCommentMarkdownGenerator.MSBuild\bin\Debug\PxtlCa.XmlCommentMarkDownGenerator.MSBuild.xml";
            var docPath = @"..\..\Docs";
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
    }
}
