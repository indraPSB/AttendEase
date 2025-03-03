using AttendEase.DB.Contexts;
using AttendEase.DB.Models;
using AttendEase.Shared.Models;
using AttendEase.Shared.Services;
using ContactDBService = AttendEase.DB.Services.ContactService;

internal class ContactService(ILogger<ContactService> logger, AttendEaseDbContext context) : IContactService
{
    private readonly ILogger<ContactService> _logger = logger;
    private readonly AttendEaseDbContext _context = context;

    public Task<IEnumerable<Contact>?> GetContacts(CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        return ContactDBService.GetContacts(_logger, _context, cancellationToken);
    }

    public Task<Contact?> GetContact(Guid id, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        return ContactDBService.GetContact(id, _logger, _context, cancellationToken);
    }

    public async Task<bool> AddContact(Contact contact, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        contact.Id = Guid.NewGuid();
        contact.Status = ContactStatus.Open;

        if (contact.Subject == ContactSubject.ResetPassword)
        {
            // Reset password for user & send email to user
            contact.MessageUser = null;
            contact.MessageSystem = $"Password was reset for user '{contact.Email}' on '{contact.Timestamp:G}'.";
            contact.Status = ContactStatus.Close;
        }

        bool result = await ContactDBService.AddContact(_logger, _context, contact, cancellationToken);

        return result;
    }

    public Task<bool> UpdateContact(Contact contact, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        return ContactDBService.UpdateContact(_logger, _context, contact, cancellationToken);
    }

    public Task<bool> DeleteContact(Guid id, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        return ContactDBService.DeleteContact(_logger, _context, id, cancellationToken);
    }

    public Task<bool> DeleteContacts(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        return ContactDBService.DeleteContacts(_logger, _context, ids, cancellationToken);
    }
}
