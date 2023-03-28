using Asp.Versioning;
using EventsApi.Structuring;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("AppConnString") ?? "Data Source=MinimalEvents.db";
builder.Services.AddSqlite<MinimalEventsDbContext>(connectionString);

builder.Services.AddDbContext<MinimalEventsDbContext>(options => options.UseSqlite("MinimalEvents"));
builder.Services.AddScoped<EventsRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRateLimiter(rateLimiterOptions =>
{
        //add a policy to create a partitioned rate limiter which limits by api-ley
    //common pool for non-api-key requests
    rateLimiterOptions.AddPolicy("fixedwindowpolicy", context => //delegate takes HttpContext and returns a RateLimitPartition
    {
        var apiKey = context.Request.Headers["api-key"];

        //no userid - basic plan gets 5 permits in 20 seconds
        if (string.IsNullOrEmpty(apiKey))
        {
            // rateLimiterOptions.RejectionStatusCode = StatusCodes.Status418ImATeapot;
            return RateLimitPartition.GetFixedWindowLimiter("Anon", options =>
            new FixedWindowRateLimiterOptions
            {
                Window = TimeSpan.FromSeconds(20),
                PermitLimit = 5,
                QueueLimit = 0,
                QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst
            });
        }

        //with the api key in header 10 requests in 10s for every api key(partition)
        return RateLimitPartition.GetFixedWindowLimiter(apiKey.FirstOrDefault(), options =>
            new FixedWindowRateLimiterOptions
            {
                Window = TimeSpan.FromSeconds(10),
                PermitLimit = 10,
                QueueLimit = 2,
                QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst
            });

    });
});

builder.Services.AddApiVersioning(options =>
    {
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.DefaultApiVersion = new ApiVersion(1);
        options.ReportApiVersions = true;
        options.ApiVersionReader = ApiVersionReader.Combine(new HeaderApiVersionReader("api-version"), new QueryStringApiVersionReader("api-version"));
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRateLimiter();

var versionedApis = app.NewVersionedApi();

var v1Version = versionedApis.MapGroup("/events").HasApiVersion(1);

var eventsGroup = v1Version.MapGroup("/");
var paidGroup = eventsGroup.MapGroup("/").AddEndpointFilter<ApiKeyFilter>();

eventsGroup.MapEventsRouteGroup();
paidGroup.MapPaidEventsRouteGroup();

var v2Version = versionedApis.MapGroup("/events").HasApiVersion(2);
var eventsV2Group = v2Version.MapGroup("/");
var paidV2Group = eventsV2Group.MapGroup("/").AddEndpointFilter<ApiKeyFilter>();

eventsV2Group.MapEventsRouteGroupV2();
paidV2Group.MapPaidEventsRouteGroupV2();



app.Run();

    