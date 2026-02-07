using Mensageria.Domain.Interfaces.Messages;
using Mensageria.Domain.Interfaces.Repositories;
using Mensageria.Domain.Interfaces.Services;
using Mensageria.Domain.Services;
using Mensageria.Infra.Message.Publishers;
using Mensageria.InfraData.Contexts;
using Mensageria.InfraData.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IArchiveRepositories, ArchiveRepositories>();
builder.Services.AddTransient<IArchiveServices, ArchiveServices>();
builder.Services.AddTransient<ISendArchiveMessage, SendArchivePublisher>();

//context
builder.Services.AddDbContext<DataContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
