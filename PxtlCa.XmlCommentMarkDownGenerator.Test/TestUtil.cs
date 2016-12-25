using System.IO;
using System.Reflection;

namespace PxtlCa.XmlCommentMarkDownGenerator.Test
{
    internal static class TestUtil
    {
        internal static string FetchResourceAsString(string resourceName)
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
