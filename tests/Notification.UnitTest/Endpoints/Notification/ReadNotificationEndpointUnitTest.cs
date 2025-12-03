using System.Net;
using Forum.Notification.Base;
using Forum.Notification.Factories;
using Forum.Notification.Fixtures;

namespace Forum.Notification.Endpoints.Notification;

public class ReadNotificationEndpointUnitTest(TestFixture fixture) : BaseTest(fixture)
{
    [Fact]
    public async Task ReadNotificationEndpoint_ShouldReadNotification()
    {
        var notification = MakeNotification.Create();
        Context.Notification.Add(notification);
        await Context.SaveChangesAsync();

        var request = MakeNotification.ReadNotificationRequest(
            notification.Id.ToString(),
            notification.RecipientId.ToString());
        var response = await DoPost("/notification/read", request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}