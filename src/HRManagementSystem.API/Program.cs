using HRManagementSystem.Application.Interfaces.Repositories;
using HRManagementSystem.Application.Interfaces.Services;
using HRManagementSystem.Application.Services;
using HRManagementSystem.Infrastructure.Data;
using HRManagementSystem.Infrastructure.Repositories;
using HRManagementSystem.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using AspNetCoreRateLimit;  



var builder = WebApplication.CreateBuilder(args);


//---------Add services to container-----

//---------API Versioning-----
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("x-api-version"),
        new MediaTypeApiVersionReader("x-api-version")
    );
});



builder.Services.AddVersionedApiExplorer(setup =>
{
    setup.GroupNameFormat = "'v'VVV";
    setup.SubstituteApiVersionInUrl = true;
});



//---- RATE LIMITING----------
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();



//------------Add controllers--------------
builder.Services.AddControllers();



//--------Add Swagger with JWT,API Versioning-------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    //V1
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "HR Management System API v1",
        Description = "HR Management System API - Version 1",
        Contact = new OpenApiContact
        {
            Name = "Himalayan Tech",
            Email = "info@himalayantech.com.np"
        }
    });
    
    //V2 for leave management
    options.SwaggerDoc("v2", new OpenApiInfo
    {
        Version = "v2",
        Title = "HR Management System API v2",
        Description = "HR Management System API - Version 2",
    });

    //Add JWT Authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below. Example: 'Bearer 12345abcdef'",
        Name = "Authorization",
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
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});



//--------Register DapperContext for db access----------
builder.Services.AddSingleton<DapperContext>();



//--------Register Repositories------
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IDesignationRepository, DesignationRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();



//-------Register Services--------------------------------------
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IDesignationService, DesignationService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();



//--------Configure JWT Auth---------
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured in appsettings.json");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero // Remove default 5 minute tolerance
    };
});



//----------Add Authorization----------
builder.Services.AddAuthorization();



//----------Configure CORS (for frontend applications)----------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();



//-----------Configure the HTTP request pipeline------------------



//----------Enable Swagger ONLY in development------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "HR Management API v1");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "HR Management API v2");
        options.RoutePrefix = string.Empty; // Makes Swagger UI the root page
    });
}

app.UseHttpsRedirection();



//----------Use CORS (before Authentication and Authorization)----------
app.UseCors("AllowAll");

//---------IP RATE LIMITING (before Authentication)----------
app.UseIpRateLimiting();

//---------Use Authentication (must be before Authorization)----------
app.UseAuthentication();

//---------Use Authorization----------
app.UseAuthorization();

// Map controller endpoints
app.MapControllers();

app.Run();