using banking_api_repo.Data;
using banking_api_repo.Interface;
using banking_api_repo.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using banking_api_repo.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddScoped<IAccountsService, AccountsServices>();
builder.Services.AddScoped<ICurrencyServices, CurrencyService>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.Configure<CurrencyApiSettings>(
    builder.Configuration.GetSection("CurrencyApi"));
builder.Services.AddHttpClient<ICurrencyServices, CurrencyService>(client =>
{
    client.BaseAddress = new Uri( "https://api.freecurrencyapi.com/v1/latest");
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AccountsContext>(options => 
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), 
        new MySqlServerVersion(new Version(8, 0, 29))));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Bank Application API", Version = "v1" });
});

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