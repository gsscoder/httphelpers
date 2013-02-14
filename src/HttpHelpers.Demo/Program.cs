/*
 * Easy HTTP Sample Server
 * Version 0.0.0.4 (based on HttpHelpers 0.1.0.19-alfa)
 * Giacomo Stelluti Scala (gsscoder@gmail.com)
 * Demonstrates use of https://github.com/gsscoder/httphelpers (work in progress)
 * How to execute: Copy & paste, then add a reference to HttpHelpers.dll
 * It handles two url:
 *  (1) http://localhost:8899/hello -> point browser & refresh
 *  (2) everything helse
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace HttpHelpers.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new Server(new IPAddress(new byte[] {0, 0, 0, 0}), 8899);
            server.Start();

            Console.ReadKey(); // We suspend the local thread

            server.Stop();
        }
    }

    static class StreamExtensions
    {
        public static IEnumerable<byte> ToByteArray(this NetworkStream stream)
        {
            var buffer = new List<byte>(1024);
            while (stream.DataAvailable)
            {
                buffer.Add((byte)stream.ReadByte());
            }
            return buffer;
        }
    }

    public class Request : IHttpParserCallbacks
    {
        public string Method = string.Empty;
        public string Uri = string.Empty;
        public string Version = string.Empty;

        public Dictionary<string, string> Headers;

        public void OnMessageBegin()
        {
            Headers = new Dictionary<string, string>();
        }

        public void OnRequestLine(string method, string uri, string version)
        {
            Method = method;
            Uri = uri;
            Version = version;
        }

        public void OnResponseLine(string version, int? code, string reason)
        {
            throw new NotImplementedException();
        }

        public void OnHeaderLine(string name, string value)
        {
            Headers.Add(name, value);
        }

        public void OnBody(ArraySegment<byte> data)
        {
            // We don't need the body for this sample, so we can simply left the callback unimplemented
        }

        public void OnMessageEnd()
        {
        }
    }

    public class Server
    {
        public Server(IPAddress localEp, int port)
        {
            _listener = new TcpListener(localEp, port);
            Trace.WriteLine("listening port " + port);
        }

        public void Start()
        {
            _listener.Start();
            HandleSocket(_listener);
        }

        public void Stop()
        {
            _listener.Stop();
        }

        private void HandleSocket(TcpListener listener)
        {
            var acceptTask = Task.Factory.FromAsync<Socket>(
                listener.BeginAcceptSocket,
                listener.EndAcceptSocket, _listener);

            acceptTask.ContinueWith(task =>
                {
                    HandleConnection(task.Result);
                    HandleSocket(listener);
                }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        private static async void HandleConnection(Socket socket)
        {
            Trace.WriteLine("client accepted");

            var ns = new NetworkStream(socket);

            //var request = Request.FromByteArray(ns.ToByteArray());
            var request = new Request();
            var parsing = HttpParser.ParseRequestAsync(new ByteArrayCharStream(ns.ToByteArray()), request);
            Trace.WriteLine("  parsing request async");
            await parsing;

            if (request.Method.ToUpperInvariant() == "GET" &&
                request.Uri.StartsWith("/hello", StringComparison.InvariantCultureIgnoreCase))
            {
                WriteResponse(ns,
                    "HTTP/1.1 200 OK\r\nContent-Type: text/html\r\n\r\n" +
                    "<html><body><p>Hello, at " + DateTime.Now.ToLongTimeString() +
                    "</p></body></html>\r\n");
            }
            else
            {
                WriteResponse(ns,
                    "HTTP/1.1 404 Not Found\r\nContent-Type: text/html\r\n\r\n" +
                    "<html><body><p>Sorry! Resource not found.</p></body></html>\r\n");
            }
            try
            {
                ns.Close();
                socket.Close();
            }
            catch (Exception e)
            {
                Trace.WriteLine("Trouble: " + e.Message);
            }
        }

        private static void WriteResponse(NetworkStream ns, string buffer)
        {
            var bytes = Encoding.UTF8.GetBytes(buffer);
            ns.Write(bytes, 0, bytes.Length);
        }

        private readonly TcpListener _listener;
    }
}

#region App.config
/*
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <startup> 
        <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.5" />
    </startup>
    <system.diagnostics>
        <trace autoflush="false" indentsize="4">
            <listeners>
                <add name="configConsoleListener"
                    type="System.Diagnostics.ConsoleTraceListener" />
            </listeners>
        </trace>
    </system.diagnostics>    
</configuration>
*/
#endregion