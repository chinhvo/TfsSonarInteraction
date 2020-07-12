using System;
using System.Collections.Generic;
using System.Text;

namespace PRInteraction
{
    public class AppSettings
    {
        public string RepoId { get; set; }
        public string ServerUrl { get; set; }
        public string PAT { get; set; }        
        public string SonarIssueUrl { get; set; }
        public string Collection { get; set; }        
        public string ProjectName { get; set; }
        public int MaxResult { get; set; }
        public string SonarServer { get; set; }
        public string SonarKey { get; set; }        
        public string SonarUserName { get; set; }
        public string SonarUserPassword { get; set; }
        public int PullRequestId { get; set; }
        public string ProjectKey { get; set; }
        public byte FirstComparingIteration { get; set; }
        public byte SecondComparingIteration { get; set; }

    }
}
