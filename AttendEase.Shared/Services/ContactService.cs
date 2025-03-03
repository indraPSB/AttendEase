using AttendEase.DB.Models;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace AttendEase.Shared.Services;

public interface IContactService
{
    Task<IEnumerable<Contact>?> GetContacts(CancellationToken cancellationToken = default);

    Task<Contact?> GetContact(Guid id, CancellationToken cancellationToken = default);

    Task<bool> AddContact(Contact contact, CancellationToken cancellationToken = default);

    Task<bool> UpdateContact(Contact contact, CancellationToken cancellationToken = default);

    Task<bool> DeleteContact(Guid id, CancellationToken cancellationToken = default);

    Task<bool> DeleteContacts(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
}

public class ContactService(ILogger<ContactService> logger, HttpClient httpClient) : IContactService
{
    private readonly ILogger<ContactService> _logger = logger;
    private readonly HttpClient _httpClient = httpClient;

    public async Task<IEnumerable<Contact>?> GetContacts(CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync("/api/contacts");

            if (response.IsSuccessStatusCode)
            {
                IEnumerable<Contact>? contacts = await response.Content.ReadFromJsonAsync<IEnumerable<Contact>>(cancellationToken);

                if (contacts is null)
                {
                    _logger.LogWarning("No contacts found.");
                }
                else
                {
                    _logger.LogInformation("Contacts retrieved successfully.");
                }

                return contacts;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Shared GetContacts with message, '{message}'.", ex.Message);
        }

        return null;
    }

    public async Task<Contact?> GetContact(Guid id, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"/api/contacts/{id}");

            if (response.IsSuccessStatusCode)
            {
                Contact? contact = await response.Content.ReadFromJsonAsync<Contact>(cancellationToken);

                if (contact is null)
                {
                    _logger.LogWarning("No contact found.");
                }
                else
                {
                    _logger.LogInformation("Contact retrieved successfully.");
                }

                return contact;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Shared GetContact with message, '{message}'.", ex.Message);
        }

        return null;
    }

    public async Task<bool> AddContact(Contact contact, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/api/contacts", contact, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Contact added successfully.");

                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Shared AddContact with message, '{message}'.", ex.Message);
        }

        return false;
    }

    public async Task<bool> UpdateContact(Contact contact, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"/api/contacts/{contact.Id}", contact, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Contact updated successfully.");

                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Shared UpdateContact with message, '{message}'.", ex.Message);
        }

        return false;
    }

    public async Task<bool> DeleteContact(Guid id, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"/api/contacts/{id}", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Contact deleted successfully.");

                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Shared DeleteContact with message, '{message}'.", ex.Message);
        }

        return false;
    }

    public async Task<bool> DeleteContacts(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/api/contacts/delete", ids, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Contacts deleted successfully.");

                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Shared DeleteContacts with message, '{message}'.", ex.Message);
        }

        return false;
    }
}
