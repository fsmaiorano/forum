using System.Net;
using Forum.Notification.Base;
using Forum.Notification.Factories;
using Forum.Notification.Fixtures;

namespace Forum.Notification.Endpoints.Notification;

public class SendNotificationEndpointUnitTest(TestFixture fixture) : BaseTest(fixture)
{
    [Fact]
    public async Task SendNotificationEndpoint_ShouldSendNotification()
    {
        var request = MakeNotification.SendNotificationRequest();
        var response = await DoPost("/notification/send", request);
        
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}