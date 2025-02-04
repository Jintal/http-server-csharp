using System.Text;
using System.IO.Compression;
using Utility;
using Utility.Enums;

namespace Models
{
    public class HttpResponse
    {
        public string Version { get; set; } = HttpVersions.HTTP_1_1;
        public int StatusCode { get; set; } = (int)StatusCodes.OK;
        public string Phrase { get; set; } = Constants.OK;
        public string[] Headers { get; set; } = [];
        public string Body { get; set; } = string.Empty;
        public bool AllowCompression { get; set; } = false;

        public byte[] GetBytes()
        {
            string headers = string.Empty;

            if (!string.IsNullOrEmpty(Body) && AllowCompression)
            {
                byte[] compressedBody = CompressBody();

                Headers = [
                    .. Headers,
                    $"{HttpHeaders.CONTENT_ENCODING}: {CompressionSchmes.GZIP}",
                    $"{HttpHeaders.CONTENT_LENGTH}: {compressedBody.Length}"
                ];
                headers = string.Join(Constants.CRLF, Headers) + (Headers.Length > 0 ? Constants.CRLF : string.Empty);

                string compressedResponse = $"{Version} {StatusCode} {Phrase}{Constants.CRLF}{headers}{Constants.CRLF}";
                return [..Encoding.UTF8.GetBytes(compressedResponse), ..compressedBody];
            }

            if (!string.IsNullOrEmpty(Body) && !AllowCompression)
            {
                Headers = [
                    .. Headers,
                    $"{HttpHeaders.CONTENT_LENGTH}: {Body.Length}"
                ];
                headers = string.Join(Constants.CRLF, Headers) + (Headers.Length > 0 ? Constants.CRLF : string.Empty);
            }
            
            string response = $"{Version} {StatusCode} {Phrase}{Constants.CRLF}{headers}{Constants.CRLF}{Body}";
            return Encoding.UTF8.GetBytes (response);
        }

        private byte[] CompressBody ()
        {
            byte[] bytes = Encoding.UTF8.GetBytes(Body);

            using MemoryStream memoryStream = new ();
            using GZipStream gzipStream = new (memoryStream, CompressionMode.Compress, true);

            gzipStream.Write(bytes, 0, bytes.Length);
            gzipStream.Flush();
            gzipStream.Close();

            return memoryStream.ToArray();
        }
    }
}

