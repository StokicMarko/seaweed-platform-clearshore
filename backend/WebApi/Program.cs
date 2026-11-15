using GrpcRepositories;
using GrpcRepositories.Generated;
using RepositoryContracts;
using WebApi.ErrorHandling;
using Services;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();

Uri dataServerAddress = new(builder.Configuration["DataServer:Address"]!);
builder.Services.AddGrpcClient<MediaData.MediaDataClient>(options => options.Address = dataServerAddress);
builder.Services.AddGrpcClient<MachineData.MachineDataClient>(options => options.Address = dataServerAddress);
builder.Services.AddGrpcClient<FileData.FileDataClient>(options => options.Address = dataServerAddress);                      
builder.Services.AddGrpcClient<ChangeRequestData.ChangeRequestDataClient>(options => options.Address = dataServerAddress);   

builder.Services.AddScoped<IMediaRepository, MediaGrpcRepository>();
builder.Services.AddScoped<IMachineRepository, MachineGrpcRepository>();
builder.Services.AddScoped<IFileRepository, FileGrpcRepository>();                      
builder.Services.AddScoped<IChangeRequestRepository, ChangeRequestGrpcRepository>();   

builder.Services.Configure<MediaUploadOptions>(builder.Configuration.GetSection(MediaUploadOptions.SectionName));  
builder.Services.Configure<CurrentUserOptions>(builder.Configuration.GetSection(CurrentUserOptions.SectionName)); 

builder.Services.AddScoped<MediaFileValidator>();                        
builder.Services.AddScoped<ICurrentUser, ConfiguredCurrentUser>();      //CHANGE WITH LOGIN  
builder.Services.AddScoped<IMediaSubmissionService, MediaSubmissionService>();

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
