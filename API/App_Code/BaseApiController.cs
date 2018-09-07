﻿using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using Newtonsoft.Json;
using System.Text;
using System.Web.Http.ModelBinding;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Entities = Agrishare.Core.Entities;
using Agrishare.Core;
using System.IO;

namespace Agrishare.API
{
    public abstract class BaseApiController : ApiController
    {
        private static string _logAPI { get; set; }
        private static bool LogAPI
        {
            get
            {
                if (_logAPI.IsEmpty())
                    _logAPI = Entities.Config.Find(Key: "Log API").Value;
                return _logAPI.Equals("True", StringComparison.InvariantCultureIgnoreCase);
            }
        }

        protected Entities.User CurrentUser
        {
            get
            {
                if (!Request.Properties.TryGetValue("CurrentUser", out object currentUser))
                    return new Entities.User();
                else
                    return (Entities.User)currentUser;
            }
        }

        public object Error(string Message)
        {
            if (LogAPI)
            {
                new Entities.Log
                {
                    Description = Log(Message),
                    LevelId = Entities.LogLevel.Log,
                    Title = HttpContext.Current.Request.Path,
                    User = CurrentUser.Title
                }.Save();
            }

            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, Message);
        }

        public object Error(ModelStateDictionary ModelState)
        {
            if (LogAPI)
            {
                new Entities.Log
                {
                    Description = Log(JsonConvert.SerializeObject(ModelState)),
                    LevelId = Entities.LogLevel.Log,
                    Title = HttpContext.Current.Request.Path,
                    User = CurrentUser.Title
                }.Save();
            }

            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        }

        public object Success(object ResponseData = null)
        {
            if (LogAPI)
            {
                new Entities.Log
                {
                    Description = Log(JsonConvert.SerializeObject(ResponseData)),
                    LevelId = Entities.LogLevel.Log,
                    Title = HttpContext.Current.Request.Path,
                    User = CurrentUser.Title
                }.Save();
            }

            if (ResponseData is string)
                return Request.CreateResponse(HttpStatusCode.OK, new { Message = ResponseData });

            if (ResponseData == null)
                return Request.CreateResponse(HttpStatusCode.OK, new { });

            return Request.CreateResponse(HttpStatusCode.OK, ResponseData);
        }

        protected async Task<Dictionary<string, object>> MultipartFormData()
        {
            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            var provider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);

            var formParams = new Dictionary<string, object>();
            foreach (var content in provider.Contents)
            {
                var isFile = content.Headers.ContentType != null && !content.Headers.ContentType.MediaType.StartsWith("text");
                var paramName = content.Headers.ContentDisposition.Name.Replace("\"", "");

                if (isFile)
                {
                    var fileData = await content.ReadAsByteArrayAsync();
                    var fileName = Entities.File.Save(content.Headers.ContentDisposition.FileName.Replace("\"", ""), fileData);
                    formParams[paramName] = fileName;
                }
                else
                    formParams[paramName] = await content.ReadAsStringAsync();
            }

            return formParams;
        }

        public int UTCOffset
        {
            get
            {
                int.TryParse(HttpContext.Current.Request.Headers["UTCOffset"], out int offset);
                return offset;
            }
        }

        private string Log(string Response)
        {
            var req = HttpContext.Current.Request;
            var requestData = new StringBuilder();

            // REQUEST
            requestData.AppendLine("-ENDPOINT-");
            requestData.AppendLine(req.Path);
            requestData.AppendLine("");

            // HEADERS
            if (req.Headers.Count > 0)
            {
                requestData.AppendLine("-HEADERS-");
                foreach (var key in req.Headers.AllKeys)
                    requestData.AppendLine($"{key}: {req.Headers[key]}");
                requestData.AppendLine("");
            }

            // FORM FIELDS
            if (req.Form.Count > 0)
            {
                requestData.AppendLine("-FORM-");
                foreach (var key in req.Form.AllKeys)
                    requestData.AppendLine($"{key}: {req.Form[key]}");
                requestData.AppendLine("");
            }

            // FILES
            if (req.Files.Count > 0)
            {
                requestData.AppendLine("-FILES-");
                foreach (var key in req.Files.AllKeys)
                    requestData.AppendLine($"{key}: {req.Files[key].FileName} ({Entities.File.FormatFileSize(req.Files[key].ContentLength)})");
                requestData.AppendLine("");
            }

            // QUERY STRING
            if (req.QueryString.Count > 0)
            {
                requestData.AppendLine("-QUERYSTRING-");
                foreach (var key in req.QueryString.AllKeys)
                    requestData.AppendLine($"{key}: {req.QueryString[key]}");
                requestData.AppendLine("");
            }

            // REQUEST BODY
            if (req.ContentType.Equals("application/json", StringComparison.InvariantCultureIgnoreCase) && Request.Method == HttpMethod.Post)
            {
                string documentContents;
                using (var receiveStream = HttpContext.Current.Request.InputStream)
                    using (var readStream = new StreamReader(receiveStream, Encoding.UTF8))
                        documentContents = readStream.ReadToEnd();

                requestData.AppendLine("-REQUEST BODY-");
                requestData.AppendLine(documentContents);
                requestData.AppendLine("");
            }

            // RESPONSE
            requestData.AppendLine("-RESPONSE-");
            requestData.AppendLine(Response);
            requestData.AppendLine("");

            return requestData.ToString();
        }
    }
}