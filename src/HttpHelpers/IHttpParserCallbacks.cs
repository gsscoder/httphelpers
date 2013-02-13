using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace HttpHelpers
{
    public interface IHttpParserCallbacks
    {
        void OnMessageBegin();
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#", Justification = "By design.")]
        void OnRequestLine(string method, string uri, string version);
        void OnResponseLine(string version, int? code, string reason);
        void OnHeaderLine(string name, string value);
        void OnBody(ArraySegment<byte> data);
        void OnMessageEnd();
    }
}