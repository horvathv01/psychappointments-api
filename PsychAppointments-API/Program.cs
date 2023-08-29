using Microsoft.AspNetCore.Authentication.Cookies;
using PsychAppointments_API.Auth;
using PsychAppointments_API.Models;
using PsychAppointments_API.Service;
using PsychAppointments_API.Service.Factories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(option =>
{
    option.AddPolicy("default", policy =>
    {
        policy.WithOrigins("192.168.1.248", "192.168.1.242")
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
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });
// Add services to the container.
builder.Services.AddScoped<IAccessUtilities, AccessUtilities>();
builder.Services.AddScoped<IHasherFactory, HasherFactory>();
builder.Services.AddTransient<IClientService, ClientService>();
builder.Services.AddTransient<IPsychologistService, PsychologistService>();
builder.Services.AddTransient<ISessionService, SessionService>();
builder.Services.AddTransient<ISlotService, SlotService>();
builder.Services.AddTransient<IUserService, UserService>();

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

//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();