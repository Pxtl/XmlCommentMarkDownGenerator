using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PxtlCa.XmlCommentMarkDownGenerator.MSBuild.Options
{
    public class SourceDocument
    {
        public YamlOptions Options { get; set; }
        public string FileName { get; set; }
        public string Body { get; set; }
    }
}
