using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace PxtlCa.XmlCommentMarkDownGenerator.MSBuild.Options
{
    /// <summary>
    /// Specifies the manner in which custom tags will be handled
    /// </summary>
    public enum AllowedTagOptions
    { 
        /// <summary>
        /// All custom tags are allowed
        /// </summary>
        All,
        /// <summary>
        /// No custom tags are allowed
        /// </summary>
        None
    }
    /// <summary>
    /// The options to be deserialized from the front matter found.
    /// </summary>
    public class YamlOptions
    {
        [YamlMember(Alias = "MergeXmlComments", ApplyNamingConventions = false)]
        public bool MergeXmlComments { get; set; }
        [YamlMember(Alias = "AllowedCustomTags", ApplyNamingConventions = false)]
        public string AllowedCustomTags { get; set; }
    }
}
