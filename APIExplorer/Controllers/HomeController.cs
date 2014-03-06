using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace APIExplorer.Controllers
{
    public class HomeController : Controller
    {
        string _recordUrl = "/v3/records";
        string _getRecordUrl = "/v3p/records";

        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }

        [Authorize]
        public ActionResult All()
        {
            if ((string)Session["token"] == null)
            {
                ViewBag.error = "Token is missing. Please [re]login again";

                return View();
            }

            try
            {
                HttpWebRequest request;

                if (Request.Params["id"] != null)
                    request = CreateRequest("get", _getRecordUrl + "/" + Request.Params["id"]);
                else
                    request = CreateRequest("get", _recordUrl);

                var httpResponse = (HttpWebResponse)request.GetResponse();
                var result = "";

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();

                    var jt = JToken.Parse(result);
                    string formattedResult = jt.ToString(Formatting.Indented);

                    ViewBag.result = formattedResult;
                }
            }
            catch(Exception ex)
            {
                ViewBag.error = ex.Message;
            }

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
            var request = CreateRequest("post", _recordUrl);

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
    }
}
