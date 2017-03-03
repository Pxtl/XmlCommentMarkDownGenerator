using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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

        private static Dictionary<string, string> _MemberNamePrefixDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
            ["F:"] = "Field",
            ["P:"] = "Property",
            ["T:"] = "Type",
            ["E:"] = "Event",
            ["M:"] = "Method",
        };

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
                    string expandedName = null;
                    if(!_MemberNamePrefixDict.TryGetValue(el.Attribute("name").Value.Substring(0,2), out expandedName))
                    {
                        expandedName = "none";
                    }
                    name = expandedName.ToLowerInvariant();
                }
                if (name == "see")
                {
                    var anchor = el.Attribute("cref") != null && el.Attribute("cref").Value.StartsWith("!:#");
                    name = anchor ? "seeAnchor" : "seePage";
                }
                //treat first Param element separately to add table headers.
                if (name.EndsWith("param")
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

        private static readonly Regex _PrefixReplacerRegex = new Regex(@"(^[A-Z]\:)");

        internal static string[] ExtractNameAndBodyFromMember(XElement node, string assemblyName)
        {
            var newName = Regex.Replace(node.Attribute("name").Value, $@":{Regex.Escape(assemblyName)}\.", ":"); //remove leading namespace if it matches the assembly name
            //TODO: do same for function parameters
            newName = _PrefixReplacerRegex.Replace(newName, match => _MemberNamePrefixDict[match.Value] + " "); //expand prefixes into more verbose words for member.
            return new[]
               {
                    newName,
                    node.Nodes().ToMarkDown(assemblyName)
                };
        }

        internal static string[] ExtractNameAndBody(string att, XElement node, string assemblyName)
        {
            return new[]
               {
                    node.Attribute(att)?.Value,
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
