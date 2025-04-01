using DDDStateMachineExample.Application.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationLayerServices();

var app = builder.Build();

app.MapControllers();
app.Run();
