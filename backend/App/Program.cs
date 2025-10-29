using App.ServiceExtensions;
using DB;
using Shared;
using Auth;
using Market;
using Shared.Services;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Serilog;
using System.Threading.RateLimiting;

#region Legacy
using RegistryApp.DB;
using RegistryApp.Services;
using Serilog.Templates;
using Db;
using Serilog.Events;
using Market.ServiceExtensions;
#endregion


var builder = WebApplication.CreateBuilder(args);

builder.Configuration.SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

// add support mvc functionality and finding all application controllers
builder.Services.AddMvc()
// overide default validation model of controller action 
    .ConfigureApiBehaviorOptions(opt => {
        opt.InvalidModelStateResponseFactory = actionContext => {
            var validationErrors = actionContext.ModelState.Values
                .SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToList();
            return new BadRequestObjectResult(new ErrorResponse {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation",
                Detail = string.Join(";\n", validationErrors),
            });
        };
    });

// lowercase paths
builder.Services.Configure<RouteOptions>(opt => {
    opt.LowercaseUrls = true;
    opt.LowercaseQueryStrings = true;
});

#region Legacy
builder.Services.AddScoped<PostgreSqlAdapter>();
builder.Services.AddScoped<ISoftwareService, SoftwareService>();
builder.Services.AddScoped<IVendorService, VendorService>();
builder.Services.AddScoped<ShortLinkService>();
#endregion

builder.Configuration.AddUserSecrets("e4bac9c2-607a-4744-b415-df707c5bae14");
builder.Services.AddDataAccess(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddRazorPages();
builder.Services.AddHttpClient();

// configure identity and auth cookie
builder.Services.AddAuthServices(builder.Environment);
builder.Services.AddScoped<InvitationService>();

// application and module's services
builder.Services.AddTransient<IHtmlParser, HtmlParser>();
builder.Services.AddTransient<EmailSender>();


//builder.Services.AddAutoMapper(opt => opt.AddProfile<MappingProfile>());
builder.Services.AddMediatR(opt => {
    opt.RegisterServicesFromAssembly(typeof(Program).Assembly);
    opt.RegisterServicesFromAssembly(typeof(Market.Market).Assembly);
    opt.RegisterServicesFromAssembly(typeof(AuthController).Assembly);
});

// global exception handler with problem details
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<CustomExceptionHandler>();

// application health checks (for "/health" end-point)
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>();

// global limits for form options
builder.Services.Configure<FormOptions>(opt => {
    opt.MultipartBodyLengthLimit = 128 * 1024 * 1024; // 128 MB (default)
});

// logging
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithProcessId()
    .Enrich.WithProcessName()
    .Enrich.WithEnvironmentVariable("GIT_BRANCH", "GitBranch")
    .Enrich.WithEnvironmentVariable("GIT_COMMIT", "GitCommit")
    .WriteTo.Console()
    .CreateBootstrapLogger();

builder.Services.AddSerilog((services, lc) => {
    lc.ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithProcessId()
        .Enrich.WithProcessName()
        .Enrich.WithEnvironmentVariable("GIT_BRANCH", "GitBranch")
        .Enrich.WithEnvironmentVariable("GIT_COMMIT", "GitCommit");

    // TODO: check Serilog API
    lc.Filter.ByExcluding((log) => {
        if (log.Properties.TryGetValue("RequestPath", out var pathValue) && pathValue is ScalarValue scalar) {
            var path = scalar.Value?.ToString();

            bool shouldExclude = path is not null && (
                path.StartsWith("/app") || path.StartsWith("/css") || path.StartsWith("/js")
            );

            return shouldExclude;
        }

        return false;
    });

    if (builder.Environment.IsDevelopment()) {
        lc.WriteTo.Console();
    } else {
        lc.WriteTo.Console(
           new ExpressionTemplate(
           "{ {" +
           "trace_id: @tr, timestamp: @t, message: @m, level: @l, " +
           "git: { commit: @p['GitCommit'], branch: @p['GitBranch'] }, " +
           "program: { pid: @p['ProcessId'], name: @p['ProcessName'] }, " +
           "fields: { @x, ..Rest()}" +
           "}}\n"
           )
       );
    }
}
);

// limits
builder.Services.AddRateLimiter(opt => {
    opt.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    opt.AddFixedWindowLimiter(policyName: "fixed", limiterOptions => {
        limiterOptions.PermitLimit = 500;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 10;
    });
});

//TODO Metrics


// cors
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", builder => {
        builder.AllowAnyOrigin();
        builder.AllowAnyMethod();
        builder.AllowAnyHeader();
        //builder.AllowCredentials(); // set origin front-end resource
    });
});

// swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureSwagger();

builder.Services.AddMarketServices();

// validation
builder.Services.AddFluentValidation(typeof(Market.Market));

var app = builder.Build();

var logger =
    app.Services
        .GetRequiredService<ILoggerFactory>()
        .CreateLogger<Program>();

logger.LogInformation("Web application is starting");
logger.LogInformation("Checking db connection");

if (!app.Environment.IsEnvironment("Testing")) {
    using (var scope = app.Services.CreateScope()) {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }
}


// middlewares
app.UseHsts();
app.UseHttpsRedirection();

app.UseExceptionHandler();
app.UseRateLimiter();
app.UseCors("AllowAll");

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI(opt =>
        opt.SwaggerEndpoint("/swagger/v1/swagger.json", "App"));
}

app.UseAuthentication();
app.UseAuthorization();

app.AddAuthV2Endpoints();

//map end-points
app.MapHealthChecks("/health");
//TODO Metrics
//app.MapPrometheusScrapingEndpoint();
//app.Map("/", () => "You use App\nEnter /swagger/index.html for using swagger");
app.MapControllers().RequireRateLimiting("fixed");

app.UseStaticFiles();
app.UseRouting();

// ---- LEGACY ----
app.MapControllerRoute(
    name: "legacy",
    pattern: "{controller=Home}/{action=Index}/{id?}");
// ---- LEGACY END ----

app.MapRazorPages();

// TODO
app.MapWhen(context => context.Request.Path.StartsWithSegments("/app"), profileApp => {
    app.UseAuthentication();
    app.UseAuthorization();
    profileApp.UseEndpoints(endpoints => {
        endpoints.MapFallbackToFile("/app/{*path:nonfile}", "/app/index.html");
    });
});

app.AddMarketEndpoints();

app.Run();


public partial class Program { } // this needs for integration tests