using System.IO;
using System.Reflection;

namespace PxtlCa.XmlCommentMarkDownGenerator.Test.Util
{
    public static class Helper
    {
        public static string FetchResourceAsString(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static string GetRegressionTestXml()
        {
            var inputResourceName = "PxtlCa.XmlCommentMarkDownGenerator.Test.RegressionBigVariant_input.xml";
            return FetchResourceAsString(inputResourceName);
        }
    }
}
