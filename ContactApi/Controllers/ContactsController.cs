using API.Middlewares.Exceptions;
using API.Validation;
using Application.Services;
using Application.ViewModels.Contact;
using Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ContactsController : ControllerBase
    {
        private readonly IContactService _contactService;

        public ContactsController(IContactService contactService)
        {
            _contactService = contactService;
        }

        /// <summary>
        /// Cadastra um novo contato no sistema.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UnauthorizedErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ContactResponseModel>> CreateContact([FromBody] ContactRequestModel contact)
        {
            ContactValidator validator = new();
            validator.IsValid(contact);

            var response = await _contactService.Create(contact);

            return CreatedAtAction("GetContact", new { id = response.Id }, response);
        }

        /// <summary>
        /// Atualiza um cadastro no sistema.
        /// </summary>
        /// <param name="id" example="1">Id do contato a ser atualizado.</param>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UnauthorizedErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ContactResponseModel>> PutContact([FromRoute] int id, [FromBody] ContactRequestModel contact)
        {
            ContactValidator validator = new();
            validator.IsValid(contact);

            var response = await _contactService.Update(id, contact);

            if (response is null)
            {
                NotFoundException.Throw("001", "Contato não encontrado.");
            }

            return Ok(response);
        }

        /// <summary>
        /// Remove um contato do sistema.
        /// </summary>
        /// <param name="id" example="1">Id do contato a ser removido.</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(UnauthorizedErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteContact([FromRoute] int id)
        {
            var success = await _contactService.Delete(id);
            if (!success)
            {
                NotFoundException.Throw("001", "Contato não encontrado.");
            }

            return NoContent();
        }

    }
}