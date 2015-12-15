using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Net;
using System.Threading;
namespace CustomIHttphandler
{
    public class Entry: IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            var request = context.Request;

            if (new HttpMethod(context.Request.HttpMethod) == HttpMethod.Post)
            {
                using (TextReader textReader = new StreamReader(request.InputStream, request.ContentEncoding))
                {
                    string content = textReader.ReadToEnd();
                    context.Response.Write("*Sync*: got your info:" + content);
                }
            }
            else if (new HttpMethod(context.Request.HttpMethod) == HttpMethod.Get)
            {
                context.Response.Write("*Sync* call is ready.");
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.Write("*Sync*: Error HTTP method:" + request.HttpMethod + ", use 'Post' instead.");
            }

            Thread.Sleep(500);

            context.Response.Flush();
            context.Response.End();
        }
    }
}