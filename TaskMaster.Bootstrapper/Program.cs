using TaskMaster.Infrastructure;
using TaskMaster.Modules.Accounts;
using TaskMaster.OpenAi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure(AppDomain.CurrentDomain.GetAssemblies().ToList(), builder.Configuration);
builder.Services.AddOpenAi();
builder.Services.AddAccounts();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseInfrastructure();
app.UseOpenAi();

app.MapControllers();

app.Run();