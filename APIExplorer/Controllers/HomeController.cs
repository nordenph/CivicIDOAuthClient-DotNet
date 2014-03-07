using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web.Mvc;

namespace APIExplorer.Controllers
{
    public class HomeController : Controller
    {
        string _createRecordUrl = "/v3/records";
        string _getAllRecordsUrl = "/v3/records";
        string _getRecordUrl = "/v3p/records";
        string _recordContactsUrl = "/v3/records/{id}/contacts";

        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }

        [Authorize]
        public ActionResult GetData(string type, string id)
        {
            if ((string)Session["token"] == null)
            {
                ViewBag.error = "Token is missing. Please [re]login again";

                return View();
            }

            var url = _getAllRecordsUrl;

            switch (type)
            {
                case "record":
                    {
                        if (id != null)
                            url = _getRecordUrl + "/" + id;

                        break;
                    }
                case "contacts":
                    {
                        if (id != null)
                            url = _recordContactsUrl.Replace("{id}", id);

                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            ViewBag.result = GetResponse("get", url);

            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult Create(string json)
        {
            var request = CreateRequest("post", _createRecordUrl);

            using (StreamWriter s = new StreamWriter(request.GetRequestStream()))
            {
                s.Write(json);
                s.Flush();
            }

            var httpResponse = (HttpWebResponse)request.GetResponse();
            
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var jt = JToken.Parse(result);
                string formattedResult = jt.ToString(Formatting.Indented);

                var id = jt["recordId"]["id"];

                ViewBag.recordId = id != null ? id.ToString() : string.Empty;
                ViewBag.result = formattedResult;
            }

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        private HttpWebRequest CreateRequest(string method, string endPoint)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://apis.accela.com" + endPoint);

            request.Method = method;
            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.Headers.Add("x-accela-appid", ConfigurationManager.AppSettings["appId"]);
            request.Headers.Add("Authorization", (string)Session["token"]);

            return request;
        }

        private string GetResponse(string method, string url)
        {
            try
            {
                HttpWebRequest request;

                request = CreateRequest(method, url);

                var httpResponse = (HttpWebResponse)request.GetResponse();
                var result = "";

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();

                    var jt = JToken.Parse(result);
                    string formattedResult = jt.ToString(Formatting.Indented);

                    return formattedResult;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
