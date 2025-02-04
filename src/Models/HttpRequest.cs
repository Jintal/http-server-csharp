using Utility;

namespace Models
{
    public class HttpRequest
    {
        public string Method { get; set; } = HttpMethods.GET;
        public string Target { get; set; } = Constants.FORWARD_SLASH;
        public string Version { get; set; } = HttpVersions.HTTP_1_1;
        public Dictionary<string, string> Headers { get; set; } = [];
        public string Body { get; set; } = string.Empty;

        public HttpRequest()
        {
        }

        public HttpRequest(string request)
        {
            Parse(request);
        }

        private void Parse(string request)
        {
            string[] requestParts = request.Split(Constants.CRLF);
            int requestPartSize = requestParts.Length;

            string[] requestLine = requestParts[0].Split(Constants.WHITESPACE);
            Method = requestLine[0];
            Target = requestLine[1];
            Version = requestLine[2];

            Headers = requestParts[1..(requestPartSize-2)]
                .ToDictionary(
                    header => header.Split($":{Constants.WHITESPACE}")[0],
                    header => header.Split($":{Constants.WHITESPACE}")[1]);

            Body = requestParts[requestPartSize-1];
        }
    }
}