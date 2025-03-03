using AttendEase.DB.Contexts;
using AttendEase.DB.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AttendEase.DB.Services;

internal static class ContactService
{
    public static async Task<IEnumerable<Contact>?> GetContacts<T>(ILogger<T> logger, AttendEaseDbContext context, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            // SELECT * FROM contact;
            return await context.Contacts.ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in DB GetContacts with message, '{message}'.", ex.Message);
        }

        return null;
    }

    public static async Task<Contact?> GetContact<T>(Guid id, ILogger<T> logger, AttendEaseDbContext context, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            // SELECT * FROM contact WHERE id = @id;
            return await context.Contacts.SingleOrDefaultAsync(c => c.Id == id, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in DB GetContact(Id) with message, '{message}'.", ex.Message);
        }

        return null;
    }

    public static async Task<bool> AddContact<T>(ILogger<T> logger, AttendEaseDbContext context, Contact contact, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            // INSERT INTO contact (id, timestamp, email, subject, message_user, message_system, status) VALUES (@id, @timestamp, @email, @subject, @messageUser, @messageSystem, @status);
            await context.Contacts.AddAsync(contact);
            await context.SaveChangesAsync(cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in DB AddContact with message, '{message}'.", ex.Message);
        }

        return false;
    }

    public static async Task<bool> UpdateContact<T>(ILogger<T> logger, AttendEaseDbContext context, Contact contact, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            // UPDATE contact SET timestamp = @timestamp, email = @email, subject = @subject, message_user = @messageUser, message_system = @messageSystem, status = @status WHERE id = @id;
            context.Contacts.Update(contact);
            await context.SaveChangesAsync(cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in DB UpdateContact with message, '{message}'.", ex.Message);
        }

        return false;
    }

    public static async Task<bool> DeleteContact<T>(ILogger<T> logger, AttendEaseDbContext context, Guid id, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            // SELECT * FROM contact WHERE id = @id;
            Contact? contact = await context.Contacts.SingleOrDefaultAsync(c => c.Id == id, cancellationToken);

            if (contact is not null)
            {
                // DELETE FROM contact WHERE id = @id;
                context.Contacts.Remove(contact);
                await context.SaveChangesAsync(cancellationToken);

                return true;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in DB DeleteContact(Id) with message, '{message}'.", ex.Message);
        }

        return false;
    }

    public static async Task<bool> DeleteContacts<T>(ILogger<T> logger, AttendEaseDbContext context, IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            // SELECT * FROM contact WHERE id IN @ids;
            IEnumerable<Contact> contacts = await context.Contacts.Where(c => ids.Contains(c.Id)).ToListAsync(cancellationToken);

            if (contacts.Any())
            {
                // DELETE FROM contact WHERE id IN @ids;
                context.Contacts.RemoveRange(contacts);
                await context.SaveChangesAsync(cancellationToken);

                return true;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in DB DeleteContacts with message, '{message}'.", ex.Message);
        }

        return false;
    }
}
