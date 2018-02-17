using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PxtlCa.XmlCommentMarkDownGenerator.MSBuild.Options
{
    public class TransformationInput
    {
        public SourceDocument SourceDocument { get; set; }
        public IEnumerable<ITaskItem> InputXml { get; set; }

        public string RelativeOutputPath
        {
            get
            {
                return this.SourceDocument.Options?.OutputFile
                    ?? $"{Path.GetFileNameWithoutExtension(this.SourceDocument.FileName)}.md";
            }
        }

        public string MergeSequence
        {
            get
            {
                return (this.SourceDocument.Options == null)
                    ? Path.GetFileName(this.RelativeOutputPath)
                    : Convert.ToString(SourceDocument.Options.MergeSequence);
            }
        }

    }
}
