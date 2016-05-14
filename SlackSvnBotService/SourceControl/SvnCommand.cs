using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SlackSvnBotService.SourceControl
{
    class SvnCommand
    {
        private const string FileName = "svn.exe";
        private const string ArgumentsFormat = "log {0} -1 1";
        private readonly Process process;
        public string Revision { get; private set; }
        public SvnCommand()
        {
            var startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = FileName,
                Arguments = string.Format(ArgumentsFormat, "https://subversion.cleverdevices.com/svnext/CleverConfig/branches/2.1.5"),
                RedirectStandardOutput = true,
                UseShellExecute = true
            };

            process = new Process
            {
                StartInfo = startInfo
            };
        }

        public Revision GetLatestRevision()
        {
            process.Start();
            return GetPayload(process.StandardOutput);
        }

        private Revision GetPayload(StreamReader output)
        {
            var currentLine = string.Empty;
            var revisionPayload = new Revision();

            while (!output.EndOfStream)
            {
                currentLine = output.ReadLine();
                if (string.IsNullOrEmpty(currentLine))
                    continue;

                var details = currentLine.Split('|').Select(x => x.Trim()).ToArray();
                if(details.Length > 1)
                {
                    revisionPayload.RevisionId = details[0];
                    revisionPayload.Server = 
                        string.Format(@"https://subversion.cleverdevices.com/svnext/CleverConfig/!svn/bc/{0}", revisionPayload.RevisionId.Replace("r", string.Empty));
                    revisionPayload.Author = details[1];

                    // skip line
                    output.ReadLine();
                    revisionPayload.Log = output.ReadLine();

                    break;
                }
            }

            return revisionPayload;
        }
    }
}
