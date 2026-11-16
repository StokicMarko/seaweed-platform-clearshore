using GrpcRepositories;
using GrpcRepositories.Generated;
using RepositoryContracts;
using WebApi.ErrorHandling;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();

Uri dataServerAddress = new(builder.Configuration["DataServer:Address"]!);
builder.Services.AddGrpcClient<MediaData.MediaDataClient>(options => options.Address = dataServerAddress);
builder.Services.AddGrpcClient<MachineData.MachineDataClient>(options => options.Address = dataServerAddress);

builder.Services.AddScoped<IMediaRepository, MediaGrpcRepository>();
builder.Services.AddScoped<IMachineRepository, MachineGrpcRepository>();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
