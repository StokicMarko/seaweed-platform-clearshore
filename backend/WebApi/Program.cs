using GrpcRepositories;
using GrpcRepositories.Generated;
using RepositoryContracts;
using WebApi.ErrorHandling;
using Services;
using Microsoft.AspNetCore.Http.Features;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();

Uri dataServerAddress = new(builder.Configuration["DataServer:Address"]!);
builder.Services.AddGrpcClient<MediaData.MediaDataClient>(options => options.Address = dataServerAddress);
builder.Services.AddGrpcClient<TechnologyData.TechnologyDataClient>(options => options.Address = dataServerAddress);
builder.Services.AddGrpcClient<FileData.FileDataClient>(options => options.Address = dataServerAddress);                      
builder.Services.AddGrpcClient<ChangeRequestData.ChangeRequestDataClient>(options => options.Address = dataServerAddress);   

builder.Services.AddScoped<IMediaRepository, MediaGrpcRepository>();
builder.Services.AddScoped<ITechnologyRepository, TechnologyGrpcRepository>();
builder.Services.AddScoped<IFileRepository, FileGrpcRepository>();                      
builder.Services.AddScoped<IChangeRequestRepository, ChangeRequestGrpcRepository>();   

builder.Services.Configure<MediaUploadOptions>(builder.Configuration.GetSection(MediaUploadOptions.SectionName));  
builder.Services.Configure<CurrentUserOptions>(builder.Configuration.GetSection(CurrentUserOptions.SectionName)); 

builder.Services.AddScoped<IMediaFileValidator, MediaFileValidator>();                     
builder.Services.AddScoped<ICurrentUser, ConfiguredCurrentUser>();      //CHANGE WITH LOGIN  
builder.Services.AddScoped<IMediaSubmissionService, MediaSubmissionService>();
builder.Services.AddScoped<IChangeRequestSubmitter, ChangeRequestSubmitter>();

// One line per target. 
builder.Services.AddScoped<IChangeRequestApplier, MediaChangeRequestApplier>();
// The registry receives every applier registered above and finds one by target type.
builder.Services.AddScoped<IChangeRequestApplierRegistry, ChangeRequestApplierRegistry>();


// The server must accept the biggest allowed file, or large videos fail  (before rules run)
MediaUploadOptions uploadOptions = builder.Configuration
    .GetSection(MediaUploadOptions.SectionName).Get<MediaUploadOptions>() ?? new MediaUploadOptions();
long maxRequestBytes = uploadOptions.LargestFileBytes + 1_048_576; // + 1 MB for the text fields (research)

builder.WebHost.ConfigureKestrel(kestrel => kestrel.Limits.MaxRequestBodySize = maxRequestBytes);
builder.Services.Configure<FormOptions>(form => form.MultipartBodyLengthLimit = maxRequestBytes);

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
