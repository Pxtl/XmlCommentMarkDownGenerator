using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

namespace PxtlCa.XmlCommentMarkDownGenerator.Test
{
    /// <summary>
    /// Summary description for ElementTests
    /// </summary>
    [TestClass]
    public class ElementTests
    {
        public ElementTests()
        {

        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void ElementPara()
        {
            var inputResourceName = "PxtlCa.XmlCommentMarkDownGenerator.Test.ElementPara_input.xml";
            Regex normalizeSpace = new Regex(@"\s+", RegexOptions.Compiled);
            var testInput = TestUtil.FetchResourceAsString(inputResourceName);

            var testOutput = normalizeSpace.Replace(testInput.ToMarkDown(), " ");
            //TODO: better test here
        }

        [TestMethod]
        public void ElementC()
        {
            var inputResourceName = "PxtlCa.XmlCommentMarkDownGenerator.Test.ElementC_input.xml";
            Regex normalizeSpace = new Regex(@"\s+", RegexOptions.Compiled);
            var testInput = TestUtil.FetchResourceAsString(inputResourceName);

            var testOutput = normalizeSpace.Replace(testInput.ToMarkDown(), " ");
            Assert.IsTrue(testOutput.Contains("`code tag c`"));
        }

        [TestMethod]
        public void ElementParam()
        {
            var inputResourceName = "PxtlCa.XmlCommentMarkDownGenerator.Test.ElementParam_input.xml";
            Regex normalizeSpace = new Regex(@"\s+", RegexOptions.Compiled);
            var testInput = TestUtil.FetchResourceAsString(inputResourceName);

            var testOutput = normalizeSpace.Replace(testInput.ToMarkDown(), " ");
        }
    }
}
