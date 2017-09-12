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

            var inputPath = @"bin\Debug\PxtlCa.XmlCommentMarkDownGenerator.MSBuild.xml";
            //get relative path here
            //1. get current directory
            //2. find PxtlCa.XmlCommentMarkDownGenerator.MSBuild.xml in the file system.
            //3. adjust relative path until File.Exists(inputPath)

            //then adjust the path and doc path, output file, etc.
            var docPath = @"..\Docs";
            var outputFile = new TaskItem("Readme.md");

            
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
        }
    }
}
