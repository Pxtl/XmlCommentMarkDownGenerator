using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PxtlCa.XmlCommentMarkDownGenerator.Test.Util
{
    public class TestWarningLogger : IWarningLogger
    {
        public List<string> WarningList { get; private set; } = new List<string>();
        public void LogWarning(string warning)
        {
            WarningList.Add(warning);
        }
    }
}
