using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PxtlCa.XmlCommentMarkDownGenerator
{
    /// <summary>
    /// Specifies the manner in which unexpected tags will be handled
    /// </summary>
    public enum UnexpectedTagActionEnum
    {
        /// <summary>
        /// No unexpected tags are allowed
        /// </summary>
        Error,
        /// <summary>
        /// Warn on unexpected tags
        /// </summary>
        Warn,
        /// <summary>
        /// All unexpected tags are allowed
        /// </summary>
        Accept
    }
}
