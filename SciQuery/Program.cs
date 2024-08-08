using Bogus;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SciQuery.Domain.UserModels;
using SciQuery.Domain.UserModels.AppRoles;
using SciQuery.Infrastructure.Persistance.DbContext;
using SciQuery.Middlewares;
using SciQuery.Service.Interfaces;
using SciQuery.Service.Mappings;
using SciQuery.Service.Services;
using Serilog;
using System.Text;

Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File("logs/logs_.txt", rollingInterval: RollingInterval.Day)
            .WriteTo.File("logs/error_.txt", Serilog.Events.LogEventLevel.Error, rollingInterval: RollingInterval.Day)
            .WriteTo.File("logs/error_.txt", Serilog.Events.LogEventLevel.Fatal, rollingInterval: RollingInterval.Day)
            .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSingleton<FileExtensionContentTypeProvider>();

//Add Services
builder.Services.AddScoped<IFileManagingService, FileMangingService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IQuestionService, QuestionService>();
builder.Services.AddScoped<IAnswerService, AnswerService>();
builder.Services.AddScoped<IVoteService, VoteService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddAutoMapper(typeof(UserMappings).Assembly);
//Identity Usermanager and rolemanager
builder.Services.AddDbContext<SciQueryDbContext>();

//UserManager class + 
builder.Services.AddIdentityCore<User>(options =>
{
    //Stringly password policy
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;

    //strongly password 
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;

    //DDOS attacks security
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
})
 .AddRoles<IdentityRole>() // To Roles
 .AddEntityFrameworkStores<SciQueryDbContext>() // Entity
 .AddDefaultTokenProviders();
//AddAuthentication


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {

        var secret = builder.Configuration["JwtConfig:SecretKey"];
        var issuer = builder.Configuration["JwtConfig:ValidIssuer"];
        var audience = builder.Configuration["JwtConfig:ValidAudiences"];
        if (secret is null || issuer is null || audience is null)
        {
            throw new ApplicationException("Jwt is not set in the Configuration");
        }
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
        };
    });

//Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

///Add auth definition Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Jwt authorization header using the Bearer scheme. Example :\"Authorization : Bearer {token}\"",
        Name = "Authoriation",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer",
                }
            }
            ,new string[]{}
        }
    });

});

//Policies for Role requirements 
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminstratorRole", policy => policy.RequireRole(AppRoles.Administrator));
    options.AddPolicy("RequireUserRole", policy => policy.RequireRole(AppRoles.User));
    options.AddPolicy("RequireMasterRole", policy => policy.RequireRole(AppRoles.Master));

    // Master is user .
    options.AddPolicy("RequireUserAndMasterRole", policy => policy.RequireRole(AppRoles.Administrator, AppRoles.User));
});

//To Allowed to client React. Without this code Api does not response to fronted.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost5173",
        builder => builder
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<SciQueryDbContext>();

    DatabaseSeeder.SeedData(context);
}

// Check if the roles exist, if not, create them
using (var serviceScope = app.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;

    // Ensure the database is created.
    var dbContext = services.GetRequiredService<SciQueryDbContext>();
    dbContext.Database.EnsureCreated();

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    if (!await roleManager.RoleExistsAsync(AppRoles.User))
    {
        await roleManager.CreateAsync(new IdentityRole(AppRoles.User));
    }

    if (!await roleManager.RoleExistsAsync(AppRoles.Administrator))
    {
        await roleManager.CreateAsync(new IdentityRole(AppRoles.Administrator));
    }

    if (!await roleManager.RoleExistsAsync(AppRoles.Master))
    {
        await roleManager.CreateAsync(new IdentityRole(AppRoles.Master));
    }
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandler>();
       
//For api response to fronted react
app.UseRouting();
//and this
app.UseCors("AllowLocalhost5173");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
