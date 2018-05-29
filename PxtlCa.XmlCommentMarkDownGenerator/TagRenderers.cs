using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace PxtlCa.XmlCommentMarkDownGenerator
{
    public class TagRenderer
    {
        public TagRenderer(string formatString, Func<XElement, ConversionContext, IEnumerable<string>> valueExtractor)
        {
            FormatString = formatString;
            ValueExtractor = valueExtractor;
        }

        public string FormatString { get; } = "";

        public Func<
            XElement, //xml Element to extract from 
            ConversionContext, //context
            IEnumerable<string> //resultant list of values that will get used with formatString
        > ValueExtractor;

        public static Dictionary<string, TagRenderer> Dict { get; } = new Dictionary<String, TagRenderer>()
        {
            ["doc"] = new TagRenderer(
                "# {0} #\n\n{1}\n\n",
                (x, context) => new[]{
                        x.Element("assembly").Element("name").Value,
                        x.Element("members").Elements("member").ToMarkDown(context.MutateAssemblyName(x.Element("assembly").Element("name").Value))
                }
            ),
            ["br"] = new TagRenderer(
                "\n",
                (x, context) => new string[0]
            ),
            ["seealso"] = new TagRenderer(
                "##### See also: {0}\n",
                (x, context) => XmlToMarkdown.ExtractNameAndBody("cref", x, context)
            ),
            ["type"] = new TagRenderer(
                "## {0}\n\n{1}\n\n---\n",
                (x, context) => XmlToMarkdown.ExtractNameAndBodyFromMember(x, context)
            ),
            ["field"] = new TagRenderer(
                "#### {0}\n\n{1}\n\n---\n",
                (x, context) => XmlToMarkdown.ExtractNameAndBodyFromMember(x, context)
            ),
            ["property"] = new TagRenderer(
                "#### {0}\n\n{1}\n\n---\n",
                (x, context) => XmlToMarkdown.ExtractNameAndBodyFromMember(x, context)
            ),
            ["method"] = new TagRenderer(
                "#### {0}\n\n{1}\n\n---\n",
                (x, context) => XmlToMarkdown.ExtractNameAndBodyFromMember(x, context)
            ),
            ["event"] = new TagRenderer(
                "#### {0}\n\n{1}\n\n---\n",
                (x, context) => XmlToMarkdown.ExtractNameAndBodyFromMember(x, context)
            ),
            ["summary"] = new TagRenderer(
                "{0}\n\n",
                (x, context) => new[] { x.Nodes().ToMarkDown(context) }
            ),
            ["value"] = new TagRenderer(
                "**Value**: {0}\n\n",
                (x, context) => new[] { x.Nodes().ToMarkDown(context) }
            ),
            ["remarks"] = new TagRenderer(
                "\n\n>{0}\n\n",
                (x, context) => new[] { x.Nodes().ToMarkDown(context) }
            ),
            ["example"] = new TagRenderer(
                "##### Example: {0}\n\n",
                (x, context) => new[] { x.Nodes().ToMarkDown(context) }
            ),
            ["para"] = new TagRenderer(
                "{0}\n\n",
                (x, context) => new[] { x.Nodes().ToMarkDown(context) }
            ),
            ["code"] = new TagRenderer(
                "\n\n###### {0} code\n\n```\n{1}\n```\n\n",
                (x, context) => new[] { x.Attribute("lang")?.Value ?? "", x.Value.ToCodeBlock() }
            ),
            ["seePage"] = new TagRenderer(
                "[[{1}|{0}]]",
                (x, context) => XmlToMarkdown.ExtractNameAndBody("cref", x, context)
            ),
            ["seeAnchor"] = new TagRenderer(
                "[{1}]({0})]",
                (x, context) => { var xx = XmlToMarkdown.ExtractNameAndBody("cref", x, context); xx[0] = xx[0].ToLower(); return xx; }
            ),
            ["firstparam"] = new TagRenderer(
                "|Name | Description |\n|-----|------|\n|{0}: |{1}|\n",
                (x, context) => XmlToMarkdown.ExtractNameAndBody("name", x, context)
            ),
            ["typeparam"] = new TagRenderer(
                "|{0}: |{1}|\n",
                (x, context) => XmlToMarkdown.ExtractNameAndBody("name", x, context)
            ),
            ["param"] = new TagRenderer(
                "|{0}: |{1}|\n",
                (x, context) => XmlToMarkdown.ExtractNameAndBody("name", x, context)
            ),
            ["paramref"] = new TagRenderer(
                "`{0}`",
                (x, context) => XmlToMarkdown.ExtractNameAndBody("name", x, context)
            ),
            ["exception"] = new TagRenderer(
                "[[{0}|{0}]]: {1}\n\n",
                (x, context) => XmlToMarkdown.ExtractNameAndBody("cref", x, context)
            ),
            ["returns"] = new TagRenderer(
                "**Returns**: {0}\n\n",
                (x, context) => new[] { x.Nodes().ToMarkDown(context) }
            ),
            ["c"] = new TagRenderer(
                " `{0}` ",
                (x, context) => new[] { x.Nodes().ToMarkDown(context) }
            ),
            ["none"] = new TagRenderer(
                "",
                (x, context) => new string[0]
            ),
        };
    }

    
}
