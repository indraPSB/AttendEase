using AttendEase.DB.Models;
using AttendEase.Shared.Models;
using AttendEase.Shared.Services;
using Microsoft.AspNetCore.Authorization;

namespace AttendEase.Web.Endpoints;

internal static class ContactEndpoint
{
    public static WebApplication MapContactEndpoints(this WebApplication app)
    {
        app.MapGet("api/contacts", GetContacts)
            .RequireAuthorization(new AuthorizeAttribute { Roles = $"{UserRole.Admin},{UserRole.Business}" });

        app.MapGet("api/contacts/{id:guid}", GetContact)
            .RequireAuthorization(new AuthorizeAttribute { Roles = $"{UserRole.Admin},{UserRole.Business}" });

        app.MapPost("api/contacts", AddContact)
            .RequireAuthorization(new AuthorizeAttribute { Roles = $"{UserRole.Admin},{UserRole.Business},{UserRole.Standard}" });

        app.MapPut("api/contacts", UpdateContact)
            .RequireAuthorization(new AuthorizeAttribute { Roles = $"{UserRole.Admin},{UserRole.Business}" });

        app.MapDelete("api/contacts/{id:guid}", DeleteContact)
            .RequireAuthorization(new AuthorizeAttribute { Roles = $"{UserRole.Admin},{UserRole.Business}" });

        app.MapPost("api/contacts/delete", DeleteContacts)
            .RequireAuthorization(new AuthorizeAttribute { Roles = $"{UserRole.Admin},{UserRole.Business}" });

        return app;
    }

    public static async Task<IResult> GetContacts(IContactService contactService, CancellationToken cancellationToken)
    {
        IEnumerable<Contact>? contacts = await contactService.GetContacts(cancellationToken);

        if (contacts is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(contacts);
    }

    public static async Task<IResult> GetContact(Guid id, IContactService contactService, CancellationToken cancellationToken)
    {
        Contact? contact = await contactService.GetContact(id, cancellationToken);

        if (contact is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(contact);
    }

    public static async Task<IResult> AddContact(Contact contact, IContactService contactService, CancellationToken cancellationToken)
    {
        if (await contactService.AddContact(contact, cancellationToken))
        {
            return Results.Created($"/api/contacts/{contact.Id}", contact);
        }

        return Results.Ok(contact);
    }

    public static async Task<IResult> UpdateContact(Contact contact, IContactService contactService, CancellationToken cancellationToken)
    {
        if (await contactService.UpdateContact(contact, cancellationToken))
        {
            return Results.Ok(contact);
        }

        return Results.BadRequest();
    }

    public static async Task<IResult> DeleteContact(Guid id, IContactService contactService, CancellationToken cancellationToken)
    {
        if (await contactService.DeleteContact(id, cancellationToken))
        {
            return Results.NoContent();
        }

        return Results.BadRequest();
    }

    public static async Task<IResult> DeleteContacts(List<Guid> ids, IContactService contactService, CancellationToken cancellationToken)
    {
        if (await contactService.DeleteContacts(ids, cancellationToken))
        {
            return Results.NoContent();
        }

        return Results.BadRequest();
    }
}
