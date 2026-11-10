using Azure.Core.Serialization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Pg.DataverseSync.Engine.Functions.Tests.ForceTableResync;

public static class HttpTestHelper
{
    public static (HttpRequestData Request, FakeHttpResponseData Response) CreateRequest()
    {
        var services = new ServiceCollection();
        services.AddOptions<WorkerOptions>().Configure(o => o.Serializer = new JsonObjectSerializer());

        var context = Substitute.For<FunctionContext>();
        context.InstanceServices.Returns(services.BuildServiceProvider());

        var response = new FakeHttpResponseData(context);
        var request = Substitute.For<HttpRequestData>(context);
        request.CreateResponse().Returns(response);

        return (request, response);
    }
}
