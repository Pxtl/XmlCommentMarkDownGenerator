using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PxtlCa.XmlCommentMarkDownGenerator
{
    public interface IWarningLogger
    {
        void LogWarning(string warning);
    }

    public class TextWriterWarningLogger : IWarningLogger
    {
        private TextWriter _textWriter;
        public TextWriterWarningLogger(TextWriter textWriter)
        {
            _textWriter = textWriter;
        }
        public void LogWarning(string warning)
        {
            _textWriter.WriteLine("WARN: " + warning);
        }
    }
}
