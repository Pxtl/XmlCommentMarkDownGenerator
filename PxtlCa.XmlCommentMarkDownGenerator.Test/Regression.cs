using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.IO;
using PxtlCa.XmlCommentMarkDownGenerator;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace PxtlCa.XmlCommentMarkDownGenerator.Test
{
    [TestClass]
    public class Regression
    {
        [TestMethod]
        public void TestBigVariantXml()
        {
            var inputResourceName = "PxtlCa.XmlCommentMarkDownGenerator.Test.RegressionBigVariant_input.xml";
            var outputResourceName = "PxtlCa.XmlCommentMarkDownGenerator.Test.RegressionBigVariant_output.md";
            Regex normalizeSpace = new Regex(@"\s+", RegexOptions.Compiled);
            var testInput = FetchResourceAsString(inputResourceName);

            var expectedOutput = normalizeSpace.Replace(FetchResourceAsString(outputResourceName), " ");
            var actualOutput = normalizeSpace.Replace(testInput.ToMarkDown(), " ");
             Assert.AreEqual(expectedOutput, actualOutput);
        }

        private static string FetchResourceAsString(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
