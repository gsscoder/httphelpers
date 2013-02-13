using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpHelpers
{
    public interface IHttpParserCallbacks
    {
        void OnMessageBegin();
        void OnRequestLine(string method, string uri, string version);
        void OnResponseLine(string version, int? code, string reason);
        void OnHeaderLine(string name, string value);
        void OnBody(ArraySegment<byte> data);
        void OnMessageEnd();
    }
}