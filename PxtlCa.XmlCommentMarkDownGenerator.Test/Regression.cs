using Microsoft.VisualStudio.TestTools.UnitTesting;
using PxtlCa.XmlCommentMarkDownGenerator.Test.Util;
using System.Linq;
using System.Text.RegularExpressions;

namespace PxtlCa.XmlCommentMarkDownGenerator.Test
{
    [TestClass]
    public class Regression
    {
        [TestMethod]
        public void TestBigVariantXml()
        {
            var testInput = Helper.GetRegressionTestXml();
            var outputResourceName = "PxtlCa.XmlCommentMarkDownGenerator.Test.RegressionBigVariant_output.md";
            Regex normalizeSpace = new Regex(@"\s+", RegexOptions.Compiled);
            
            var expectedOutput = normalizeSpace.Replace(Helper.FetchResourceAsString(outputResourceName), " ");
            var actualOutput = normalizeSpace.Replace(testInput.ToMarkDown(), " ");
            Assert.AreEqual(expectedOutput, actualOutput);
        }
    }
}
