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
            var templates = new Dictionary<string, string>
                {
                    {"doc", "# {0} #\n\n{1}\n\n"},
                    {"type", "## {0}\n\n{1}\n\n---\n"},
                    {"field", "#### {0}\n\n{1}\n\n---\n"},
                    {"property", "#### {0}\n\n{1}\n\n---\n"},
                    {"method", "#### {0}\n\n{1}\n\n---\n"},
                    {"event", "#### {0}\n\n{1}\n\n---\n"},
                    {"summary", "{0}\n\n"},
                    {"remarks", "\n\n>{0}\n\n"},
                    {"example", "##### Example: {0}\n\n"},
                    {"para", "{0}\n\n"},
                    {"code", "\n\n###### {0} code\n\n```\n{1}\n```\n\n"},
                    {"seePage", "[[{1}|{0}]]"},
                    {"seeAnchor", "[{1}]({0})"},
                    {"param", "|Name | Description |\n|-----|------|\n|{0}: |{1}|\n" },
                    {"exception", "[[{0}|{0}]]: {1}\n\n" },
                    {"returns", "Returns: {0}\n\n"},
                    {"none", ""}
                };
            var valueExtractorsDict = new Dictionary<string, Func<XElement, IEnumerable<string>>>
                {
                    {"doc", x=> new[]{
                        x.Element("assembly").Element("name").Value,
                        x.Element("members").Elements("member").ToMarkDown(x.Element("assembly").Element("name").Value)
                    }},
                    {"type", x=>ExtractNameAndBodyFromMember("name", x, assemblyName)},
                    {"field", x=> ExtractNameAndBodyFromMember("name", x, assemblyName)},
                    {"property", x=> ExtractNameAndBodyFromMember("name", x, assemblyName)},
                    {"method",x=>ExtractNameAndBodyFromMember("name", x, assemblyName)},
                    {"event", x=>ExtractNameAndBodyFromMember("name", x, assemblyName)},
                    {"summary", x=> new[]{ x.Nodes().ToMarkDown(assemblyName) }},
                    {"remarks", x => new[]{x.Nodes().ToMarkDown(assemblyName) }},
                    {"example", x => new[]{x.Nodes().ToMarkDown(assemblyName) }},
                    {"para", x=> new[]{ x.Nodes().ToMarkDown(assemblyName) }},
                    {"code", x => new[]{x.Attribute("lang")?.Value ?? "", x.Value.ToCodeBlock()}},
                    {"seePage", x=> ExtractNameAndBody("cref", x, assemblyName) },
                    {"seeAnchor", x=> { var xx = ExtractNameAndBody("cref", x, assemblyName); xx[0] = xx[0].ToLower(); return xx; }},
                    {"param", x => ExtractNameAndBody("name", x, assemblyName) },
                    {"exception", x => ExtractNameAndBody("cref", x, assemblyName) },
                    {"returns", x => new[]{x.Nodes().ToMarkDown(assemblyName) }},
                    {"none", x => new string[0]}
                };

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

                try { 
                    var vals = valueExtractorsDict[name](el).ToArray();
                    return string.Format(templates[name], args: vals);
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

        static string ToCodeBlock(this string s)
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
