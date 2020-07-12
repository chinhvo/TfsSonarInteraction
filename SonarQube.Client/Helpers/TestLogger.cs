using SonarQube.Client.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonarQube.Client.Helpers
{
    public class TestLogger : ILogger
    {
        public List<string> DebugMessages { get; } = new List<string>();
        public List<string> ErrorMessages { get; } = new List<string>();
        public List<string> InfoMessages { get; } = new List<string>();
        public List<string> WarningMessages { get; } = new List<string>();

        public void Debug(string message)
        {
            DebugMessages.Add(message);
        }

        public void Error(string message)
        {
            ErrorMessages.Add(message);
        }

        public void Info(string message)
        {
            InfoMessages.Add(message);
        }

        public void Warning(string message)
        {
            WarningMessages.Add(message);
        }
    }
}
