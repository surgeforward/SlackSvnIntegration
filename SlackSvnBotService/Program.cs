using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Web;

namespace SlackSvnBotService
{
    class Program
    {
        static void Main(string[] args)
        {
            var timer = new Timer(1000);
            timer.Elapsed += OnTimerEvent;
            //timer.Start();
            OnTimerEvent(null, null);

            Console.WriteLine("Press enter to exit the program");
            Console.ReadLine();
        }

        private static string lastRevisionId = string.Empty;

        private static void OnTimerEvent(object source, ElapsedEventArgs args)
        {
            //Console.WriteLine("time!! {0}", args.SignalTime);
            var process = new Process();
            var startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "svn.exe";
            startInfo.Arguments = @"log https://subversion.cleverdevices.com/svnext/CleverConfig/branches/2.1.5 -l 1";
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;
            process.StartInfo = startInfo;
            process.Start();

            string currentLine = string.Empty;
            var payload = new Revision();
            while (!process.StandardOutput.EndOfStream)
            {
                currentLine = process.StandardOutput.ReadLine();
                var details = currentLine.Split('|');
                if (details.Length > 1)
                {
                    lastRevisionId = details[0].Trim();
                    payload.RevisionId = details[0].Trim();
                    payload.Server = "https://subversion.cleverdevices.com/svnext/CleverConfig/!svn/bc/11000";
                    payload.Author = details[1].Trim();
                    payload.Log = process.StandardOutput.ReadLine();
                    payload.Log = process.StandardOutput.ReadLine().Trim();

                    break;
                }
            }
            string format = "%7B%22revision%22%3A{0}%2C%22url%22%3A%22{1}%22%2C%22author%22%3A%22{2}%22%2C%22log%22%3A%22Log%20info%22%7D";

            var postData = HttpUtility.UrlEncode(JsonConvert.SerializeObject(payload));
            var data = Encoding.ASCII.GetBytes(string.Format("payload={0}", string.Format(format, 11000, string.Format(@"https://subversion.cleverdevices.com/svnext/CleverConfig/!svn/bc/{0}", payload.RevisionId.Replace("r", string.Empty)), "Test")));
            data = Encoding.ASCII.GetBytes(string.Format("payload={0}", postData));
            var request = (HttpWebRequest)WebRequest.Create(@"https://surgellc.slack.com/services/hooks/subversion?token=uGASagl3rShvSCT9VP1sXsjG");
            request.Method = "POST";
            request.ContentType = @"application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();
        }
    }   
}
