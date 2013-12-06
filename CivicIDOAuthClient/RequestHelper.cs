using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace Accela.OAuth.Client
{
    public static class RequestHelper
    {
        public static T SendPOSTRequest<T>(WebRequest request, string query)
        {
            using (Stream requestStream = request.GetRequestStream())
            {
                var writer = new StreamWriter(requestStream);
                writer.Write(query);
                writer.Flush();
            }

            var response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    var reader = new StreamReader(responseStream);

                    return JsonConvert.DeserializeObject<T>(reader.ReadToEnd());
                }
            }

            return default(T);
        }
    }
}
