using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PxtlCa.XmlCommentMarkDownGenerator.Test
{
    [TestClass]
    public class UnexpectedElement
    {
        [TestMethod]
        [ExpectedException(typeof(System.Xml.XmlException))]
        public void TestUnexpectedElementError()
        {
            var inputResourceName = "PxtlCa.XmlCommentMarkDownGenerator.Test.UnexpectedElement_input.xml";
            var testInput = Util.Helper.FetchResourceAsString(inputResourceName);

            //exception thrown below
            var testOutput = testInput.ToMarkDown();
        }

        [TestMethod]
        public void TestUnexpectedElementWarning()
        {
            var inputResourceName = "PxtlCa.XmlCommentMarkDownGenerator.Test.UnexpectedElement_input.xml";
            Regex normalizeSpace = new Regex(@"\s+", RegexOptions.Compiled);
            var testInput = Util.Helper.FetchResourceAsString(inputResourceName);
            var warningLogger = new Util.TestWarningLogger();
            var context = new ConversionContext
            {
                UnexpectedTagAction = UnexpectedTagActionEnum.Warn,
                WarningLogger = warningLogger
            };

            //exception thrown below
            var testOutput = normalizeSpace.Replace(testInput.ToMarkDown(context), " ");
            Assert.IsTrue(warningLogger.WarningList.Any(w => w.Contains("Unknown element type")));
        }
    }
}
