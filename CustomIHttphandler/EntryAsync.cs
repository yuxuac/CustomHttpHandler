using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web;

namespace CustomIHttphandler
{
    public delegate void AsyncProcessDelegate(HttpContext context);
    
    public class EntryAsync : IHttpAsyncHandler
    {
        private AsyncProcessDelegate AsyncProcess;

        public bool IsReusable
        {
            get { return false; }
        }

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {
            Console.WriteLine("BeginProcessRequest");
            AsyncProcess = new AsyncProcessDelegate(ProcessRequest);
            return AsyncProcess.BeginInvoke(context, cb, extraData);
        }

        public void EndProcessRequest(IAsyncResult result)
        {
            Console.WriteLine(result.AsyncState);
            AsyncProcess.EndInvoke(result);
        }

        public void ProcessRequest(HttpContext context)
        {
            var request = context.Request;

            if (new HttpMethod(context.Request.HttpMethod) == HttpMethod.Post)
            {
                using (TextReader textReader = new StreamReader(request.InputStream, request.ContentEncoding))
                {
                    string content = textReader.ReadToEnd();
                    context.Response.Write("*Async*: got your info:" + content);
                }
            }
            else if (new HttpMethod(context.Request.HttpMethod) == HttpMethod.Get)
            {
                context.Response.Write("*Async* call is ready.");
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.Write("*Async*: Error HTTP method:" + request.HttpMethod + ", use 'Post' instead.");
            }

            Thread.Sleep(500);
        }
    }
}