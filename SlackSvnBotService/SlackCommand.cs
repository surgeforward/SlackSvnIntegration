using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Web;

namespace SlackSvnBotService
{
    public class SlackCommand
    {
        private const string PayloadFormat = "payload={0}";
        private const string ContentTypeFormUrlEncoded = "application/x-www-form-urlencoded";
        
        public HttpWebResponse NotifyBot(Revision payload)
        {
            var encodedPayload = HttpUtility.UrlEncode(JsonConvert.SerializeObject(payload));
            var data = Encoding.ASCII.GetBytes(string.Format(PayloadFormat, encodedPayload));

            var request = (HttpWebRequest)WebRequest.Create(@"https://surgellc.slack.com/services/hooks/subversion?token=uGASagl3rShvSCT9VP1sXsjG");
            request.Method = "POST";
            request.ContentType = ContentTypeFormUrlEncoded;
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            return (HttpWebResponse)request.GetResponse();

        }
    }
}
