namespace Microex.All.AliyunOss
{
    public class AliyunOssOptions
    {
        private string _imageUrlPrefix;
        public string LocalEndPoint { get; set; } = "fileupload";
        public string RemoteEndPoint { get; set; } = "oss-cn-shanghai.aliyuncs.com";

        public string ImageUrlPrefix
        {
            get => _imageUrlPrefix;
            set => _imageUrlPrefix = value.TrimEnd('/');
        }

        public string AccessKeyId { get; set; }
        public string AccessKeySecret { get; set; }
        public string BulkName { get; set; }
        /// <summary>
        /// 20MB
        /// </summary>
        public long MaxFileSize { get; set; } = 2500000;
    }
}