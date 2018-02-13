using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PxtlCa.XmlCommentMarkDownGenerator.MSBuild
{
    internal class BuildMessageWarningLogger : IWarningLogger
    {
        private TaskLoggingHelper _log;
        internal BuildMessageWarningLogger(TaskLoggingHelper log) { _log = log; }

        public void LogWarning(string warning)
        {
            _log.LogWarning(warning);
        }
    }
}
