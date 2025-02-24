using AttendEase.DB.Contexts;
using AttendEase.DB.Models;
using AttendEase.Shared.Models;
using AttendEase.Shared.Providers;
using AttendEase.Shared.Services;
using AttendEase.Web.Components;
using AttendEase.Web.Endpoints;
using AttendEase.Web.Services;
using Bogus;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Net;
using System.Net.Security;
using System.Text;
using AttendanceWebService = AttendEase.Web.Services.AttendanceService;
using AuthWebService = AttendEase.Web.Services.AuthService;
using ScheduleWebService = AttendEase.Web.Services.ScheduleService;
using UserWebService = AttendEase.Web.Services.UserService;

const int Seed = 17;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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

builder.Services.AddDbContext<AttendEaseDbContext>(optionsBuilder =>
{
    optionsBuilder.UseNpgsql(builder.Configuration.GetConnectionString("AttendEaseDatabase"))
        .UseAsyncSeeding(async (context, _, ct) =>
        {
            #region Generate deterministic GUID v7

            static Guid GenerateGuidV7(Guid guid, long timestamp = default)
            {
                if (timestamp == default)
                {
                    // Generate a 64-bit timestamp for the first 6 bytes of the GUID
                    timestamp = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero).ToUnixTimeMilliseconds();
                }

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

            Faker<User> userFaker = new Faker<User>()
                .UseSeed(Seed)
                .RuleFor(u => u.Id, f => GenerateGuidV7(f.Random.Guid()))
                .RuleFor(u => u.Name, f => f.Person.FullName)
                .RuleFor(u => u.Password, f => f.Internet.Password())
                .RuleFor(u => u.Email, f => f.Person.Email)
                .RuleFor(u => u.Role, f => UserRole.Standard);

            List<User> users = userFaker.Generate(20);
            users[0].Role = UserRole.Admin;
            users[1].Role = UserRole.Business;

            #endregion

            #region Faker for Schedule

            Faker<Schedule> userSchedule = new Faker<Schedule>()
                .UseSeed(Seed)
                .RuleFor(s => s.Id, f => GenerateGuidV7(f.Random.Guid()));

            List<Schedule> schedules = userSchedule.Generate(2);
            schedules[0].Name = "Normal Ops";
            schedules[0].StartDate = null;
            schedules[0].EndDate = null;
            schedules[0].StartTime = new TimeOnly(9, 0, 0);
            schedules[0].EndTime = new TimeOnly(18, 0, 0);
            schedules[0].DaysOfWeek = DaysOfWeek.Weekdays.ToString();
            schedules[0].Latitude = 1.2912078M;
            schedules[0].Longitude = 103.8575408M;
            schedules[0].LocationTolerance = 50F;
            schedules[0].AttendanceStartBefore = 15;
            schedules[0].AbsentAfter = 15;
            schedules[0].Repeat = true;
            schedules[1].Name = "Feb Special Proj";
            schedules[1].StartDate = new DateOnly(2025, 2, 1);
            schedules[1].EndDate = new DateOnly(2025, 2, 28);
            schedules[1].StartTime = new TimeOnly(9, 0, 0);
            schedules[1].EndTime = new TimeOnly(13, 0, 0);
            schedules[1].DaysOfWeek = DaysOfWeek.Weekends.ToString();
            schedules[1].Latitude = null;
            schedules[1].Longitude = null;
            schedules[1].LocationTolerance = null;
            schedules[1].AttendanceStartBefore = 15;
            schedules[1].AbsentAfter = 30;
            schedules[1].Repeat = false;

            #endregion

            #region Faker for Assignment

            int count = 0;

            foreach (User user in users.Where(u => u.Role == UserRole.Standard))
            {
                if (count++ < 5)
                {
                    user.Schedules.Add(schedules[1]);
                }
                else if (count < 15)
                {
                    user.Schedules.Add(schedules[0]);
                }
                else
                {
                    user.Schedules.Add(schedules[0]);
                    user.Schedules.Add(schedules[1]);
                }
            }

            #endregion

            #region Faker for Attendance

            Faker<Attendance> attendanceFaker = new Faker<Attendance>()
                .UseSeed(Seed)
                .RuleFor(a => a.Id, f => f.Random.Guid());

            List<Attendance> attendances = [];
            IEnumerator<Attendance> attendanceEnumerator = attendanceFaker.GenerateForever().GetEnumerator();
            DateOnly date = new(2025, 1, 1);
            DateOnly endDate = new(2025, 3, 9);

            while (true)
            {
                if (date > endDate)
                {
                    break;
                }

                foreach (User user in users.Where(u => u.Role != "Admin"))
                {
                    foreach (Schedule schedule in user.Schedules)
                    {
                        attendanceEnumerator.MoveNext();

                        TimeOnly time = schedule.StartTime ?? new(0, 0, 0);
                        Attendance attendance = attendanceEnumerator.Current;
                        attendance.Timestamp = new DateTimeOffset(date, time, TimeSpan.Zero);
                        attendance.Id = GenerateGuidV7(attendance.Id, attendance.Timestamp.ToUnixTimeMilliseconds());
                        attendance.UserId = user.Id;
                        attendance.ScheduleId = schedule.Id;
                        attendance.Attended = true;
                        attendance.Schedule = schedule;
                        attendance.User = user;

                        attendances.Add(attendance);
                    }
                }

                date = date.AddDays(1);
            }

            #endregion

            #region Faker for Contact

            Faker<Contact> contactFaker = new Faker<Contact>()
                .UseSeed(Seed)
                .RuleFor(c => c.Id, f => f.Random.Guid())
                .RuleFor(c => c.Email, f => f.Person.Email)
                .RuleFor(c => c.Subject, f => f.PickRandom("General Feedback", "Others"))
                .RuleFor(c => c.MessageUser, f => f.Lorem.Paragraph())
                .RuleFor(c => c.MessageSystem, f => null)
                .RuleFor(c => c.Status, f => f.PickRandom("Open", "Close"))
                .RuleFor(c => c.Timestamp, f => f.Date.BetweenOffset(new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2025, 3, 1, 0, 0, 0, TimeSpan.Zero)));

            List<Contact> contacts = contactFaker.Generate(50);
            contacts[5].Email = users[5].Email;
            contacts[5].Subject = "Reset Password";
            contacts[5].MessageUser = null;
            contacts[5].MessageSystem = $"Password was reset for user '{users[5]}' on '{contacts[5].Timestamp:G}'.";
            contacts[15].Email = users[7].Email;
            contacts[15].Subject = "Reset Password";
            contacts[15].MessageUser = null;
            contacts[15].MessageSystem = $"Password was reset for user '{users[7]}' on '{contacts[15].Timestamp:G}'.";
            contacts[25].Email = users[9].Email;
            contacts[25].Subject = "Reset Password";
            contacts[25].MessageUser = null;
            contacts[25].MessageSystem = $"Password was reset for user '{users[9]}' on '{contacts[25].Timestamp:G}'.";
            contacts[35].Email = users[11].Email;
            contacts[35].Subject = "Reset Password";
            contacts[35].MessageUser = null;
            contacts[35].MessageSystem = $"Password was reset for user '{users[11]}' on '{contacts[35].Timestamp:G}'.";
            contacts[45].Email = users[13].Email;
            contacts[45].Subject = "Reset Password";
            contacts[45].MessageUser = null;
            contacts[45].MessageSystem = $"Password was reset for user '{users[13]}' on '{contacts[45].Timestamp:G}'.";

            #endregion

            #region Add generated fake values to DB

            bool userExist = await context.Set<User>().ContainsAsync(users[0], ct);
            if (!userExist)
            {
                await context.Set<User>().AddRangeAsync(users, ct);
                await context.Set<Schedule>().AddRangeAsync(schedules, ct);
                await context.Set<Attendance>().AddRangeAsync(attendances, ct);
                await context.Set<Contact>().AddRangeAsync(contacts, ct);
                await context.SaveChangesAsync(ct);
            }

            #endregion
        });
});

builder.Services.AddScoped<IAuthService, AuthWebService>();
builder.Services.AddScoped<IAttendanceService, AttendanceWebService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IScheduleService, ScheduleWebService>();
builder.Services.AddScoped<IUserService, UserWebService>();
builder.Services.AddScoped<AuthenticationStateProvider, AttendEaseAuthenticationStateProvider>();

builder.Services.AddBlazorBootstrap();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApi();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add device-specific services used by the AttendEase.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();

// Configure certificate validation callback
ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) =>
{
    // For local development, allow all certificates.
    if (builder.Environment.IsDevelopment())
    {
        return true;
    }

    return errors == SslPolicyErrors.None;
};

var app = builder.Build();

// Ensure the database is created
await using (AsyncServiceScope scope = app.Services.CreateAsyncScope())
{
    await using (AttendEaseDbContext context = scope.ServiceProvider.GetRequiredService<AttendEaseDbContext>())
    {
        if (await context.Database.CanConnectAsync())
        {
            //await context.Database.EnsureDeletedAsync();
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
if (!app.Environment.IsDevelopment())
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

app.MapAuthEndpoints();
app.MapAttendanceEndpoints();
app.MapScheduleEndpoints();
app.MapUserEndpoints();

app.MapOpenApi();
app.MapScalarApiReference("scalar", options =>
{
    options.WithTitle("AttendEase API Reference");
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(
        typeof(AttendEase.Shared._Imports).Assembly);

app.Run();
