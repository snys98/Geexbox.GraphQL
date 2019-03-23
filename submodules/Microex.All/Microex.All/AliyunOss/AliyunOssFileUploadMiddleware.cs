using IdentityServer4.Extensions;
using Microex.All.Extensions;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Microex.All.AliyunOss
{
    public class AliyunOssFileUploadMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AliyunOssClient _ossClient;

        public AliyunOssFileUploadMiddleware(RequestDelegate next, AliyunOssClient ossClient)
        {
            _next = next;
            _ossClient = ossClient;
        }

        public Task Invoke(HttpContext context)
        {

            if (!context.Request.Headers.ContainsKey("file") && context.Request?.HasFormContentType == false)
            {
                return _next.Invoke(context);
            }
            var resultList = new List<ViewDataUploadFilesResult>();
            if (context.Request?.HasFormContentType == false)
            {
                if (!context.Request.Headers.ContainsKey("file"))
                {
                    return _next.Invoke(context);
                }
                var dataurl = new StreamReader(context.Request.Body).ReadToEnd();
                var base64 = dataurl.DataUrlToBase64WithExt();
                var url = _ossClient.UploadBase64(dataurl);
                resultList.Add(new ViewDataUploadFilesResult()
                {
                    Name = base64.base64.ComputeMd5(),
                    Type = base64.ext,
                    Size = base64.base64.Length,
                    Url = url
                });
                return context.Response.WriteAsync(new { files = resultList }.ToJson());
            }
            var files = context.Request?.ReadFormAsync().Result.Files;
            foreach (var file in files)
            {
                var url = _ossClient.UploadPostFile(file);
                resultList.Add(new ViewDataUploadFilesResult()
                {
                    Name = file.FileName,
                    Type = file.ContentType,
                    Size = file.Length,
                    Url = url
                });
            }


            var names = files.Select(f => f.FileName);
            return context.Response.WriteAsync(new { files = resultList }.ToJson());
        }
    }
}
