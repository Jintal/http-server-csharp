namespace Utility
{
    public static class Constants
    {
        public const string CRLF = "\r\n";
        public const string OK = "OK";
        public const string NOT_FOUND = "Not Found";
        public const string WHITESPACE = " ";
        public const string FORWARD_SLASH = "/";
        public const string CREATED = "Created";
        public const string COMMA_WHITESPACE = ", ";
    }

    public static class ContentType
    {
        public const string TEXT_PLAIN = "text/plain";
        public const string APPLICATION_OCTET_STREAM = "application/octet-stream";
    }

    public static class HttpVersions
    {
        public const string HTTP_1_1 = "HTTP/1.1";
    }

    public static class HttpMethods
    {
        public const string GET = "GET";
        public const string POST = "POST";
    }

    public static class HttpHeaders
    {
        public const string USER_AGENT = "User-Agent";
        public const string CONTENT_TYPE = "Content-Type";
        public const string CONTENT_LENGTH = "Content-Length";
        public const string CONTENT_ENCODING = "Content-Encoding";
        public const string ACCEPT_ENCODING = "Accept-Encoding";
    }

    public static class Paths
    {
        public const string EMPTY = "";
        public const string ECHO = "echo";
        public const string FILES = "files";
        public const string USER_AGENT = "user-agent";
    }

    public static class CompressionSchmes
    {
        public const string GZIP = "gzip";
    }
}