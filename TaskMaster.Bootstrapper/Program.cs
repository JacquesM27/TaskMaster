using Swashbuckle.AspNetCore.ReDoc;
using TaskMaster.Infrastructure;
using TaskMaster.Modules.Accounts;
using TaskMaster.Modules.Exercises;
using TaskMaster.OpenAi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();
builder.Services.AddInfrastructure(AppDomain.CurrentDomain.GetAssemblies().ToList(), builder.Configuration);
builder.Services.AddOpenAi();
builder.Services.AddAccounts();
builder.Services.AddExercisesModule();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
    app.UseReDoc(c =>
    {
        c.DocumentTitle = "TaskMaster API";
        c.SpecUrl = "/swagger/v1/swagger.json";
        c.RoutePrefix = "docs";
        c.ConfigObject = new ConfigObject
        {
            ExpandResponses = "200"
        };
    });
}

app.UseInfrastructure();
app.UseOpenAi();

app.MapControllers();

app.Run();