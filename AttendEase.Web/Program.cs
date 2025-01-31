using AttendEase.DB.Contexts;
using AttendEase.DB.Models;
using AttendEase.Shared.Services;
using AttendEase.Web.Components;
using AttendEase.Web.Endpoints;
using AttendEase.Web.Services;
using Bogus;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UserWebService = AttendEase.Web.Services.UserService;

const int Seed = 17;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AttendEaseDbContext>(optionsBuilder =>
{
    optionsBuilder.UseNpgsql(builder.Configuration.GetConnectionString("AttendEaseDatabase"))
        .UseAsyncSeeding(async (context, _, ct) =>
        {
            #region Generate deterministic GUID v7

            static Guid GenerateGuidV7(Guid guid)
            {
                // Generate a 64-bit timestamp for the first 6 bytes of the GUID
                long timestamp = new DateTimeOffset(2025, 1, 14, 0, 0, 0, TimeSpan.Zero).ToUnixTimeMilliseconds();

                // Convert the GUID to a span for manipulation
                Span<byte> guidBytes = stackalloc byte[16];
                guid.TryWriteBytes(guidBytes);

                // Insert the timestamp into the first 6 bytes (big-endian order)
                guidBytes[0] = (byte)((timestamp >> 40) & 0xFF);
                guidBytes[1] = (byte)((timestamp >> 32) & 0xFF);
                guidBytes[2] = (byte)((timestamp >> 24) & 0xFF);
                guidBytes[3] = (byte)((timestamp >> 16) & 0xFF);
                guidBytes[4] = (byte)((timestamp >> 8) & 0xFF);
                guidBytes[5] = (byte)(timestamp & 0xFF);

                // Set the version (0b0111 for v7)
                guidBytes[6] = (byte)((guidBytes[6] & 0x0F) | 0x70);

                // Set the variant (0b10xx for RFC 4122)
                guidBytes[8] = (byte)((guidBytes[8] & 0x3F) | 0x80);

                // Return the modified GUID
                return new Guid(guidBytes);
            }

            #endregion

            #region Faker for User

            Faker<User> faker = new Faker<User>()
                   .UseSeed(Seed)
                   .RuleFor(u => u.Id, f => GenerateGuidV7(f.Random.Guid()))
                   .RuleFor(u => u.Name, f => f.Person.FullName)
                   .RuleFor(u => u.Password, f => f.Internet.Password())
                   .RuleFor(u => u.Email, f => f.Person.Email)
                   .RuleFor(u => u.Role, f => f.PickRandom("Admin", "User", "User")); // Higher chance of "User" than "Admin"

            List<User> users = faker.Generate(10);

            #endregion

            #region Add generated fake values to DB

            bool userExist = await context.Set<User>().ContainsAsync(users[0], ct);
            if (!userExist)
            {
                await context.Set<User>().AddRangeAsync(users, ct);
                await context.SaveChangesAsync(ct);
            }

            #endregion
        });
});

builder.Services.AddScoped<IUserService, UserWebService>();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddBlazorBootstrap();

// Add device-specific services used by the AttendEase.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtBearer:Issuer"] ?? throw new InvalidOperationException("The issuer is missing from the configuration."),
            ValidAudience = builder.Configuration["JwtBearer:Audience"] ?? throw new InvalidOperationException("The audience is missing from the configuration."),
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration["JwtBearer:IssuerSigningKey"] ?? throw new InvalidOperationException("The signing key is missing from the configuration.")))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Ensure the database is created
await using (AsyncServiceScope scope = app.Services.CreateAsyncScope())
{
    await using (AttendEaseDbContext context = scope.ServiceProvider.GetRequiredService<AttendEaseDbContext>())
    {
        if (await context.Database.CanConnectAsync())
        {
            await context.Database.EnsureCreatedAsync();
        }
        else
        {
            scope.ServiceProvider.GetRequiredService<ILogger<Program>>()
                .LogWarning("Cannot connect to the database.");
        }
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapUserEndpoints();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(
        typeof(AttendEase.Shared._Imports).Assembly,
        typeof(AttendEase.Web.Client._Imports).Assembly);

app.Run();
