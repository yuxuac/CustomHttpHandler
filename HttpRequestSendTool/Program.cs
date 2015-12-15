using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
namespace UploadTool
{
    class Program
    {
        private const int NumOfRequest = 30;

        /// <summary>
        /// https://msdn.microsoft.com/en-us/library/system.net.httpwebrequest.begingetresponse(v=vs.110).aspx
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //string postData = "This is a test that posts this string to a Web server.";
            string urlAsync = @"http://localhost:6191/Async";
            string urlSync = @"http://localhost:6191/Sync";
            Stopwatch watch = new Stopwatch();

            // Sync call
            watch.Start();
            for (int i = 0; i < NumOfRequest; i++)
            {
                Post(urlSync, i.ToString(), PostType.Sync);
            }
            watch.Stop();
            Console.WriteLine("Sync call:" + NumOfRequest + " requests cost:" + watch.Elapsed.TotalMilliseconds / 1000 + "s.");

            // Async call
            watch.Restart();
            for (int i = 0; i < NumOfRequest; i++)
            {
                Post(urlAsync, i.ToString(), PostType.Async);
                Console.WriteLine(i + ": Do some other work...");
            }
            watch.Stop();
            Console.WriteLine("Async call:" + NumOfRequest + " requests cost:" + watch.Elapsed.TotalMilliseconds / 1000 + "s.");
           
            Console.ReadLine();
        }

        public async static void Post(string url, string content, PostType postType)
        {
            HttpWebRequest request = GetWebRequest(url, content);

            WebResponse response = null;

            if (postType == PostType.Sync)
            {
                response = request.GetResponse();
            }
            else if (postType == PostType.Async)
            {
                response = await request.GetResponseAsync();
            }

            using (Stream responseStream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    string responseFromServer = ((HttpWebResponse)response).StatusDescription + ":" + reader.ReadToEnd();
                    Console.WriteLine(responseFromServer);
                    reader.Close();
                }
                responseStream.Close();
            }
            response.Close();
            response.Dispose();
        }

        private static HttpWebRequest GetWebRequest(string url, string content)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            byte[] byteArray = Encoding.UTF8.GetBytes(content);
            request.ContentLength = byteArray.Length;

            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
            }
            return request;
        }
    }

    public enum PostType
    { 
        Async,
        Sync
    }
}
