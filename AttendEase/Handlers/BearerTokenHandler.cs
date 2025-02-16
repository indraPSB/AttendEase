using AttendEase.Shared.Services;
using System.Net.Http.Headers;

namespace AttendEase.Handlers;

internal class BearerTokenHandler(IAuthService authService) : DelegatingHandler
{
    private readonly IAuthService _authService = authService;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string? token = await _authService.GetToken(cancellationToken);

        if (token is not null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
