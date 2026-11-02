using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Pg.DataverseSync.Engine.Functions.Tests.ForceTableResync;

public class FakeHttpResponseData : HttpResponseData
{
    public FakeHttpResponseData(FunctionContext functionContext) : base(functionContext)
    {
        Headers = new HttpHeadersCollection();
        Body = new MemoryStream();
    }

    public override HttpStatusCode StatusCode { get; set; }

    public override HttpHeadersCollection Headers { get; set; }

    public override Stream Body { get; set; }

    public override HttpCookies Cookies => throw new NotSupportedException();

    public string ReadBody()
    {
        Body.Position = 0;
        return new StreamReader(Body).ReadToEnd();
    }
}
