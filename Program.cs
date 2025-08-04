using Asp.Versioning;
using BankingAPILevel4;
using BankingAPILevel4.Data;
using BankingAPILevel4.Repositories;
using Microsoft.EntityFrameworkCore;
using BankingAPILevel4.Services;
using BankingAPILevel4.Formatters;
using BankingAPILevel4.Interfaces;

var builder = WebApplication.CreateBuilder(args).RegisterAuthentication();

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true;
    options.RespectBrowserAcceptHeader = true;
    options.OutputFormatters.Add(new CsvOutputFormatter());
});
builder.Services.AddScoped<IAuthenticationServices, AuthenticationServices>();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddScoped<IAccountsService, AccountsServices>();
builder.Services.AddScoped<ICurrencyServices, CurrencyService>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.Configure<CurrencyApiSettings>(
    builder.Configuration.GetSection("CurrencyApi"));
builder.Services.AddHttpClient<ICurrencyServices, CurrencyService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["CurrencyApi:BaseAddress"]);
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddSwagger();
builder.Services.AddDbContext<UserContext>(options => 
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), 
        new MySqlServerVersion(new Version(8, 0, 29))));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApiVersioning(setupAction =>
{
    setupAction.ReportApiVersions = true;
    setupAction.AssumeDefaultVersionWhenUnspecified = true;
    setupAction.DefaultApiVersion = new ApiVersion(1, 0);
}).AddMvc();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bank Application API V1");
});

app.MapControllers();

app.Run();