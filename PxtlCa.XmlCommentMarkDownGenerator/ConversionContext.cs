using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PxtlCa.XmlCommentMarkDownGenerator
{
    /// <summary>
    /// Helper internal class for the Node conversion
    /// </summary>
    public class ConversionContext
    {
        /// <summary>
        /// Warning log writer.  Throws exceptions if null.
        /// </summary>
        public IWarningLogger WarningLogger { get; set; }
        public string AssemblyName { get; set; }
        public ConversionContext MutateAssemblyName(string assemblyName) 
        {
            AssemblyName = assemblyName;
            return this;
        }
        public UnexpectedTagActionEnum UnexpectedTagAction { get; set; } = UnexpectedTagActionEnum.Error;
    }
}
