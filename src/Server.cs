using System.Net;
using System.Net.Sockets;
using System.Text;
using Models;
using Utility;
using Utility.Enums;

namespace Custom
{
    public class HttpServer
    {
        public static async Task Main(string[] args)
        {
            TcpListener server = new (IPAddress.Any, 4221);
            server.Start();

            while (true)
            {
                Socket socket = await server.AcceptSocketAsync();
                Task.Run(async () => await HandleSocketAsync(socket));
            }
        }

        private static async Task HandleSocketAsync(Socket socket)
        {
            try
            {
                HttpRequest request = await GetRequestFromSocket(socket);
                HttpResponse response = await ProcessRequest(request);

                await socket.SendAsync(response.GetBytes());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
        }

        private static async Task<HttpRequest> GetRequestFromSocket(Socket socket)
        {
            byte[] requestBytes = new byte[1024];
            int bytesReceived = await socket.ReceiveAsync(requestBytes);

            string requestString = Encoding.UTF8.GetString(requestBytes, 0, bytesReceived);
            HttpRequest request = new (requestString);

            return request;
        }

        private static async Task<HttpResponse> ProcessRequest(HttpRequest request)
        {
            HttpResponse response = new ();
            string[] urlParts = request.Target.Split(Constants.FORWARD_SLASH);
            
            string basePath = urlParts[1];

            switch (basePath)
            {
                case Paths.EMPTY :
                    break;
                case Paths.FILES :
                    await HandleFilePath (request, response, urlParts);
                    break;
                case Paths.ECHO :
                    HandleEchoPath (request, response, urlParts);
                    break;
                case Paths.USER_AGENT :
                    HandleUserAgentPath (request, response);
                    break;
                default :
                    HandleDefaultPath (response);
                    break;
            }

            return response;
        }

        private static async Task HandleFilePath(HttpRequest request, HttpResponse response, string[] urlParts)
        {
            string fileName = urlParts.Last();
            string filePath = $"/tmp/data/codecrafters.io/http-server-tester/{fileName}";

            switch (request.Method)
            {
                case HttpMethods.GET :
                    if (File.Exists (filePath))
                    {
                        string fileContent = await File.ReadAllTextAsync (filePath);

                        response.Headers = [
                            $"{HttpHeaders.CONTENT_TYPE}: {ContentType.APPLICATION_OCTET_STREAM}"
                        ];
                        response.Body = fileContent;
                    }
                    else
                    {
                        response.StatusCode = (int)StatusCodes.ERROR;
                        response.Phrase = Constants.NOT_FOUND;
                    }
                    break;
                case HttpMethods.POST :
                    await File.WriteAllTextAsync(filePath, request.Body);

                    response.StatusCode = (int)StatusCodes.CREATED;
                    response.Phrase = Constants.CREATED;
                    break;
            }
        }

        private static void HandleEchoPath(HttpRequest request, HttpResponse response, string[] urlParts)
        {
            string body = urlParts.Last();
            Dictionary<string, string> headers = request.Headers;
 
            headers.TryGetValue(HttpHeaders.ACCEPT_ENCODING, out string? acceptEncoding);
            string[] compressionSchemes = string.IsNullOrEmpty(acceptEncoding) ? [] : acceptEncoding.Split(Constants.COMMA_WHITESPACE);

            response.AllowCompression = compressionSchemes.Contains(CompressionSchmes.GZIP);
            response.Headers =
            [
                .. response.Headers,
                $"{HttpHeaders.CONTENT_TYPE}: {ContentType.TEXT_PLAIN}"
            ];
            response.Body = body;
        }

        private static void HandleUserAgentPath (HttpRequest request, HttpResponse response)
        {
            string body = request.Headers[HttpHeaders.USER_AGENT];

            response.Headers =
            [
                .. response.Headers,
                $"{HttpHeaders.CONTENT_TYPE}: {ContentType.TEXT_PLAIN}"
            ];
            response.Body = body;
        }

        private static void HandleDefaultPath (HttpResponse response)
        {
            response.StatusCode = (int)StatusCodes.ERROR;
            response.Phrase = Constants.NOT_FOUND;
        }
    }
}