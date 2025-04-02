using DDDStateMachineExample.Api.Extensions;
using DDDStateMachineExample.Application.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddExceptionHandlers()
    .AddApplicationLayerServices();

var app = builder.Build();

app.MapControllers();
app.Run();
