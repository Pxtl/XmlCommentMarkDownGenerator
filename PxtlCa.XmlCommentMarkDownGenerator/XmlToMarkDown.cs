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
        public static string ToMarkDown(this string s)
        {
            return s.ToMarkDown(new ConversionContext {
                UnexpectedTagAction = UnexpectedTagActionEnum.Error
                , WarningLogger = new TextWriterWarningLogger(Console.Error)
            });
        }

        public static string ToMarkDown(this string s, ConversionContext context)
        {
            var xdoc = XDocument.Parse(s);
            return xdoc
                .ToMarkDown(context)
                .RemoveRedundantLineBreaks();
        }

        public static string ToMarkDown(this Stream s)
        {
            var xdoc = XDocument.Load(s);
            return xdoc
                .ToMarkDown(new ConversionContext { UnexpectedTagAction = UnexpectedTagActionEnum.Error, WarningLogger = new TextWriterWarningLogger(Console.Error) })
                .RemoveRedundantLineBreaks();
        }

        private static Dictionary<string, string> _MemberNamePrefixDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
            ["F:"] = "Field",
            ["P:"] = "Property",
            ["T:"] = "Type",
            ["E:"] = "Event",
            ["M:"] = "Method",
        };

        /// <summary>
        /// Write out the given XML Node as Markdown. Recursive function used internally.
        /// </summary>
        /// <param name="node">The xml node to write out.</param>
        /// <param name="ConversionContext">The Conversion Context that will be passed around and manipulated over the course of the translation.</param>
        /// <returns>The converted markdown text.</returns>
        public static string ToMarkDown(this XNode node, ConversionContext context)
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
                    var vals = TagRenderer.Dict[name].ValueExtractor(el, context).ToArray();
                    return string.Format(TagRenderer.Dict[name].FormatString, args: vals);
                }
                catch(KeyNotFoundException ex)
                {
                    var lineInfo = (IXmlLineInfo)node;
                    switch(context.UnexpectedTagAction)
                    {
                        case UnexpectedTagActionEnum.Error:
                            throw new XmlException($@"Unknown element type ""{ name }""", ex, lineInfo.LineNumber, lineInfo.LinePosition);
                        case UnexpectedTagActionEnum.Warn:
                            context.WarningLogger.LogWarning($@"Unknown element type ""{ name }"" on line {lineInfo.LineNumber}, pos {lineInfo.LinePosition}");
                            break;
                        case UnexpectedTagActionEnum.Accept:
                            //do nothing;
                            break;
                        default:
                            throw new InvalidOperationException($"Unexpected {nameof(UnexpectedTagActionEnum)}");
                    }
                }
            }


            if (node.NodeType == XmlNodeType.Text)
                return Regex.Replace(((XText)node).Value.Replace(@"            ", ""), @"^\n|[S*|\n]$","",RegexOptions.Multiline);
            return "";
        }

        private static readonly Regex _PrefixReplacerRegex = new Regex(@"(^[A-Z]\:)");

        internal static string[] ExtractNameAndBodyFromMember(XElement node, ConversionContext context)
        {
            var newName = Regex.Replace(node.Attribute("name").Value, $@":{Regex.Escape(context.AssemblyName)}\.", ":"); //remove leading namespace if it matches the assembly name
            //TODO: do same for function parameters
            newName = _PrefixReplacerRegex.Replace(newName, match => _MemberNamePrefixDict[match.Value] + " "); //expand prefixes into more verbose words for member.
            return new[]
               {
                    newName,
                    node.Nodes().ToMarkDown(context)
                };
        }

        internal static string[] ExtractNameAndBody(string att, XElement node, ConversionContext context)
        {
            return new[]
               {
                    node.Attribute(att)?.Value,
                    node.Nodes().ToMarkDown(context)
                };
        }

        internal static string ToMarkDown(this IEnumerable<XNode> es, ConversionContext context)
        {
            return es.Aggregate("", (current, x) => current + x.ToMarkDown(context));
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
