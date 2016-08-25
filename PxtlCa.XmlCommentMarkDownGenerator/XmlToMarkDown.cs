using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace PxtlCa.XmlCommentMarkDownGenerator
{
    public static class XmlToMarkdown
    {
        public static string ToMarkDown(this string e)
        {
            var xdoc = XDocument.Parse(e);
            return xdoc
                .ToMarkDown()
                .RemoveRedundantLineBreaks();
        }

        public static string ToMarkDown(this Stream e)
        {
            var xdoc = XDocument.Load(e);
            return xdoc
                .ToMarkDown()
                .RemoveRedundantLineBreaks();
        }

        public static string ToMarkDown(this XNode node, string assemblyName = null)
        {
            if(node is XDocument)
            {
                node = ((XDocument)node).Root;
            }

            string name;
            if (node.NodeType == XmlNodeType.Element)
            {
                var el = (XElement)node;
                name = el.Name.LocalName;
                if (name == "member")
                {
                    switch (el.Attribute("name").Value[0])
                    {
                        case 'F': name = "field"; break;
                        case 'P': name = "property"; break;
                        case 'T': name = "type"; break;
                        case 'E': name = "event"; break;
                        case 'M': name = "method"; break;
                        default: name = "none"; break;
                    }
                }
                if (name == "see")
                {
                    var anchor = el.Attribute("cref").Value.StartsWith("!:#");
                    name = anchor ? "seeAnchor" : "seePage";
                }
                //treat first Param element separately to add table headers.
                if (name == "param"
                    && node
                        .ElementsBeforeSelf()
                        .LastOrDefault()
                        ?.Name
                        ?.LocalName != "param")
                {
                    name = "firstparam";
                }

                try { 
                    var vals = TagRenderer.Dict[name].ValueExtractor(el, assemblyName).ToArray();
                    return string.Format(TagRenderer.Dict[name].FormatString, args: vals);
                }
                catch(KeyNotFoundException ex)
                {
                    var lineInfo = (IXmlLineInfo)node;
                    throw new XmlException($@"Unknown element type ""{ name }""", ex, lineInfo.LineNumber, lineInfo.LinePosition);
                }
            }

            if (node.NodeType == XmlNodeType.Text)
                return Regex.Replace(((XText)node).Value.Replace('\n', ' '), @"\s+", " ");

            return "";
        }

        internal static string[] ExtractNameAndBodyFromMember(string att, XElement node, string assemblyName)
        {
            return new[]
               {
                    Regex.Replace(node.Attribute(att).Value, $@":{Regex.Escape(assemblyName)}\.", ":"), //remove leading namespace if it matches the assembly name
                    //TODO: do same for function parameters
                    node.Nodes().ToMarkDown(assemblyName)
                };
        }

        internal static string[] ExtractNameAndBody(string att, XElement node, string assemblyName)
        {
            return new[]
               {
                    node.Attribute(att).Value,
                    node.Nodes().ToMarkDown(assemblyName)
                };
        }

        internal static string ToMarkDown(this IEnumerable<XNode> es, string assemblyName = null)
        {
            return es.Aggregate("", (current, x) => current + x.ToMarkDown(assemblyName));
        }

        internal static string ToCodeBlock(this string s)
        {
            var lines = s.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var blank = lines[0].TakeWhile(x => x == ' ').Count() - 4;
            return string.Join("\n", lines.Select(x => new string(x.SkipWhile((y, i) => i < blank).ToArray()))).TrimEnd();
        }

        static string RemoveRedundantLineBreaks(this string s)
        {
            return Regex.Replace(s, @"\n\n\n+", "\n\n");
        }
    }
}
