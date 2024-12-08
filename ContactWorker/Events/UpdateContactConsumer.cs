using Application.Services;
using Application.ViewModels.Contact;
using Common.QueueMessageModels;
using MassTransit;

namespace ContactWorker.Events;

public class UpdateContactConsumer: IConsumer<UpdateContactMessage>
{
    private readonly IContactService _contactService;

    public UpdateContactConsumer(IContactService contactService)
    {
        _contactService = contactService;
    }

    public async Task Consume(ConsumeContext<UpdateContactMessage> context)
    {
        var id = context.Message.contactId;
        var entity = MapMessageToEntity(context.Message);

        await _contactService.Update(id, entity);

        await Task.CompletedTask;
    }
    
    private ContactRequestModel MapMessageToEntity(UpdateContactMessage message)
    {
        var contact = new ContactRequestModel
        {
            Name = message.Name,
            Email = message.Email,
            Phone = message.Phone,
            Ddd = message.Ddd,
        };
        
        return contact;
    }
}