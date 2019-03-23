using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using Aliyun.OSS;
using Aliyun.OSS.Util;
using Microex.All.Extensions;
using Microsoft.AspNetCore.Http;

namespace Microex.All.AliyunOss
{
    public class AliyunOssClient
    {
        private string _imageBulkName;
        private OssClient _ossClient;
        private AliyunOssOptions _options;

        public AliyunOssClient(AliyunOssOptions options)
        {
            this._imageBulkName = options.BulkName;
            this._ossClient = new OssClient(options.RemoteEndPoint, options.AccessKeyId, options.AccessKeySecret);
            this._options = options;
        }

        public string UploadStream(Stream stream)
        {
            var md5 = OssUtils.ComputeContentMd5(stream, stream.Length);
            var fileName = $"{md5}";
            var result = this._ossClient.PutObject(this._imageBulkName, fileName, stream, new ObjectMetadata()
            {
                ContentMd5 = md5,
            });
            return $"{_options.ImageUrlPrefix}/{fileName}";
        }

        public string UploadBase64(string dataUrl)
        {
            var base64Data = Regex.Match(dataUrl, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
            var base64WithExt = dataUrl.DataUrlToBase64WithExt();
            Stream stream = new MemoryStream(Convert.FromBase64String(base64Data));
            var md5 = OssUtils.ComputeContentMd5(stream, stream.Length);
            var fileName = HttpUtility.UrlEncode($"{md5}.{base64WithExt.ext}");
            try
            {
                var result = this._ossClient.PutObject(this._imageBulkName, $"{md5}.{base64WithExt.ext}", stream, new ObjectMetadata()
                {
                    ContentType = base64WithExt.ext,
                    ContentMd5 = md5,
                });
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e);
            }
            return $"{_options.ImageUrlPrefix}/{fileName}";
        }

        public string UploadPostFile(IFormFile formFile)
        {
            var stream = formFile.OpenReadStream();
            var fileMime = formFile.ContentType;
            if (formFile.Length > this._options.MaxFileSize)
            {
                 throw new NotSupportedException($"文件大小不能超过{this._options.MaxFileSize}");
            }
            //var fileExt = formFile.FileName.Split('.').LastOrDefault() ?? "";
            var md5 = OssUtils.ComputeContentMd5(stream, stream.Length);
            var fileName = HttpUtility.UrlEncode(formFile.FileName);
            try
            {
                var result = this._ossClient.PutObject(this._imageBulkName, formFile.FileName, stream, new ObjectMetadata()
                {
                    ContentType = fileMime,
                    ContentMd5 = md5,
                });
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e);
            }
            return $"{_options.ImageUrlPrefix}/{fileName}";
        }

    }
}