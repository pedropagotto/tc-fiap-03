using Application.Services;
using Application.ViewModels.Contact;
using Common.QueueMessageModels;
using Domain.Entities;
using MassTransit;

namespace ContactWorker.Events;

public class CreateContactConsumer: IConsumer<CreateContactMessage>
{
    private readonly IContactService _contactService;

    public CreateContactConsumer(IContactService contactService)
    {
        _contactService = contactService;
    }

    public async Task Consume(ConsumeContext<CreateContactMessage> context)
    {
        var entity = MapMessageToEntity(context.Message);

        await _contactService.Create(entity);

        await Task.CompletedTask;
    }

    private ContactRequestModel MapMessageToEntity(CreateContactMessage message)
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