using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using PsychAppointments_API.Auth;
using PsychAppointments_API.DAL;
using PsychAppointments_API.Models;
using PsychAppointments_API.Service;
using PsychAppointments_API.Service.DataProtection;
using PsychAppointments_API.Service.Factories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("http://localhost:3000", "http://192.168.1.248:3000")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        options.Cookie.Name = "PsychAppointmentsCookie";
        options.Cookie.SameSite = SameSiteMode.None;
        //works in https only:
        //options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

builder.Services.AddControllers().AddNewtonsoftJson();


// Add services to the container.
builder.Services.AddDbContext<PsychAppointmentContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("PsychAppointmentsConnection")));

builder.Services.AddScoped<IAccessUtilities, AccessUtilities>();
builder.Services.AddScoped<IHasherFactory, HasherFactory>();

builder.Services.AddSingleton<IRepository<User>, InMemoryUserRepository>();
builder.Services.AddSingleton<IRepository<Location>, InMemoryLocationRepository>();
builder.Services.AddSingleton<IRepository<Session>, InMemorySessionRepository>();
builder.Services.AddSingleton<IRepository<Slot>, InMemorySlotRepository>();

builder.Services.AddTransient<IClientService, ClientService>();
builder.Services.AddTransient<IPsychologistService, PsychologistService>();
builder.Services.AddTransient<ISessionService, SessionService>();
builder.Services.AddTransient<ISlotService, SlotService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IManagerService, ManagerService>();
builder.Services.AddTransient<ILocationService, LocationService>();

//prepopulate DB and/or in memory repositories via interface for testing purposes
builder.Services.AddScoped<IPrepopulate, Prepopulate>();

builder.Services.AddSingleton<IDataProtectionService<Admin>, AdminDataProtectionService>();
builder.Services.AddSingleton<IDataProtectionService<Client>, ClientDataProtectionService>();
builder.Services.AddSingleton<IDataProtectionService<Manager>, ManagerDataProtectionService>();
builder.Services.AddSingleton<IDataProtectionService<Psychologist>, PsychologistDataProtectionService>();
builder.Services.AddSingleton<IDataProtectionService<User>, DataProtectionService>();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();