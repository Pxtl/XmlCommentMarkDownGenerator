using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace PxtlCa.XmlCommentMarkDownGenerator.MSBuild.Options
{
    /// <summary>
    /// The options to be deserialized from the front matter found.
    /// </summary>
    public class YamlOptions
    {
        [YamlMember(Alias = "MergeXmlComments", ApplyNamingConventions = false)]
        public bool MergeXmlComments { get; set; }
        [YamlMember(Alias = "AllowedCustomTags", ApplyNamingConventions = false)]
        public string UnexpectedTagAction { get; set; }
        //[YamlMember(Alias = "Namespaces", ApplyNamingConventions = false)]
        //public string Namespaces { get; set; }
        [YamlMember(Alias = "OutputFile", ApplyNamingConventions = false)]
        public string OutputFile { get; set; }
    }
}
