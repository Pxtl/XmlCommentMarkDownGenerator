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
        [YamlMember(Alias = nameof(UnexpectedTagAction), ApplyNamingConventions = false)]
        public string UnexpectedTagAction { get; set; }

        [YamlMember(Alias = nameof(MergeSequence), ApplyNamingConventions = false)]
        public int MergeSequence { get; set; }

        //[YamlMember(Alias = nameof(AssemblyNames), ApplyNamingConventions = false)]
        //public string AssemblyNames { get; set; }

        [YamlIgnore]
        public UnexpectedTagActionEnum UnexpectedTagActionAsEnum
        {
            get
            {
                if (Enum.TryParse<UnexpectedTagActionEnum>(UnexpectedTagAction, true,
                        out UnexpectedTagActionEnum result))
                {
                    return result;
                }
                else
                {
                    return UnexpectedTagActionEnum.Error;
                }
            }
        }
        
        //[YamlMember(Alias = "Namespaces", ApplyNamingConventions = false)]
        //public string Namespaces { get; set; }
        [YamlMember(Alias = nameof(OutputFile), ApplyNamingConventions = false)]
        public string OutputFile { get; set; }
    }
}
