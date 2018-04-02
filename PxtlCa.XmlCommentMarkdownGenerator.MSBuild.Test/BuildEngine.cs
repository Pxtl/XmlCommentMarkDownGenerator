using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Build.Framework;
using Microsoft.Build.Tasks;
using Microsoft.Build.Utilities;
using PxtlCa.XmlCommentMarkDownGenerator;
using PxtlCa.XmlCommentMarkDownGenerator.MSBuild;
using Rhino.Mocks;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml;
using System.Linq;

namespace PxtlCa.XmlCommentMarkDownGenerator.MSBuild.Test
{
    [TestClass]
    public class TestMerges
    {
        [TestMethod]
        public void ExecuteDefaultMerge()
        {
            var defaultMergeSearchText = "DefaultMergeSearchHeaderText";
            PrepareInputDocsDirectory("Notes", $@"
---
---
## {defaultMergeSearchText}
".Trim());
            PrepareStandardOutputDocsDirectory();
            var mockRepo = new MockRepository();
            var buildEngine = mockRepo.Stub<IBuildEngine>();

            var xmlFileName = "DefaultMerge.xml";
            File.WriteAllText(xmlFileName, PxtlCa.XmlCommentMarkDownGenerator.Test.Util.Helper.GetRegressionTestXml());
            
            var task = new GenerateMarkdown
            {
                BuildEngine = buildEngine,
                SourceDocumentDirPath = new TaskItem(InputDocsDirectory),
                TargetDocumentDirPath = new TaskItem(OutputDocsDirectory),
                InputXml = new ITaskItem[] { new TaskItem(xmlFileName) }
            };

            task.Execute();

            var fileActuallyExists = Directory.EnumerateFiles(OutputDocsDirectory)
                .Any(f => File.ReadAllText(f).Contains(defaultMergeSearchText));

            Assert.IsTrue(fileActuallyExists);
        }

        [TestMethod]
        public void HandleUnexpectedTag()
        {
            PrepareStandardInputDocsDirectory();
            PrepareStandardOutputDocsDirectory();
            var mockRepo = new MockRepository();
            var buildEngine = mockRepo.Stub<IBuildEngine>();

            var toAlter = PxtlCa.XmlCommentMarkDownGenerator.Test.Util.Helper.GetRegressionTestXml();
            toAlter = toAlter.Replace(@"<returns>", @"<returns><inheritdoc>X</inheritdoc>");

            var alteredPath = @"MSBuild_alteredWarning.xml";
            File.WriteAllText(alteredPath, toAlter);

            var task = new GenerateMarkdown
            {
                BuildEngine = buildEngine,
                SourceDocumentDirPath = new TaskItem(InputDocsDirectory),
                TargetDocumentDirPath = new TaskItem(OutputDocsDirectory),
                InputXml = new ITaskItem[] { new TaskItem(alteredPath) },
                UnexpectedTagAction = UnexpectedTagActionEnum.Accept
            };

            task.Execute();
            if (null != task.LoggedException)
            {
                throw task.LoggedException;
            }
        }


        [TestMethod]
        [ExpectedException(typeof(XmlException))]
        public void ExpectedErrorUnexpectedTag()
        {
            PrepareStandardInputDocsDirectory();
            PrepareStandardOutputDocsDirectory();
            var mockRepo = new MockRepository();
            var buildEngine = mockRepo.Stub<IBuildEngine>();

            var toAlter = PxtlCa.XmlCommentMarkDownGenerator.Test.Util.Helper.GetRegressionTestXml();
            toAlter = toAlter.Replace(@"<returns>", @"<returns><inheritdoc>X</inheritdoc>");

            var alteredPath = @"MSBuild_alteredError.xml";
            File.WriteAllText(alteredPath, toAlter);
                        
            var task = new GenerateMarkdown
            {
                BuildEngine = buildEngine,
                SourceDocumentDirPath = new TaskItem(InputDocsDirectory),
                TargetDocumentDirPath = new TaskItem(OutputDocsDirectory),
                InputXml = new ITaskItem[] { new TaskItem(alteredPath) },
                UnexpectedTagAction = XmlCommentMarkDownGenerator.UnexpectedTagActionEnum.Error
            };

            task.Execute();

            if (null != task.LoggedException)
            {
                throw task.LoggedException;
            }
        }

        public static void PrepareStandardOutputDocsDirectory()
        {
            PrepareDirectory(OutputDocsDirectory);
        }
        public static void PrepareStandardInputDocsDirectory()
        {
            PrepareInputDocsDirectory("Notes", @"
---
---
## No Documentation Yet Authored
");
        }

        public static readonly string OutputDocsDirectory = "OutputDocs";
        public static readonly string InputDocsDirectory = "InputDocs";
               
        public static void PrepareInputDocsDirectory(string mdDocFileNameSansExtension, string mdDocFileBody)
        {
            PrepareDirectory(InputDocsDirectory);
            File.WriteAllText($@"{InputDocsDirectory}\{mdDocFileNameSansExtension}.md", mdDocFileBody.Trim());
        }

        private static void PrepareDirectory(string directoryName)
        {
            if (Directory.Exists(directoryName))
            {
                Directory.Delete(directoryName, true);
            }
            Directory.CreateDirectory(directoryName);
        }
    }
}
