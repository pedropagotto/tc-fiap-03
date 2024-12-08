using Application.Services;
using Application.ViewModels.Contact;
using Common.QueueMessageModels;
using MassTransit;

namespace ContactWorker.Events;

public class DeleteContactConsumer: IConsumer<RemoveContactMessage>
{
    private readonly IContactService _contactService;

    public DeleteContactConsumer(IContactService contactService)
    {
        _contactService = contactService;
    }
    
    public async Task Consume(ConsumeContext<RemoveContactMessage> context)
    {
        var id = context.Message.contactId;

        await _contactService.Delete(id);

        await Task.CompletedTask;
    }
}