using Forum.Application.EventHandlers;

namespace Forum.Application.HostedServices;

public class EventHandlerSubscriptionService(
    OnAnswerCreated onAnswerCreated,
    OnQuestionBestAnswerChosen onQuestionBestAnswerChosen)
    : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        onAnswerCreated.SetupSubscriptions();
        onQuestionBestAnswerChosen.SetupSubscriptions();

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}