using Microsoft.EntityFrameworkCore;
using SampleWebAPI.Services;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<AppDbContext>(otp =>
    otp.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Services.AddSwaggerGen();
// builder.Services.AddSwaggerGen(options =>
// {
//     // กำหนด SecurityScheme สำหรับ Bearer token
//     options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//     {
//         Name = "Authorization",  // header name 
//         Type = SecuritySchemeType.ApiKey, // defind apiKey bc Bearer use it in header req
//         In = ParameterLocation.Header, // defind in header
//         Scheme = "Bearer", // defind use Bearer
//         BearerFormat = "JWT", // use jwt
//         Description = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwiaWQiOiIxNyIsIm5iZiI6MTczMTY2NDQ4NCwiZXhwIjoxNzMxNjY4MDg0LCJpYXQiOjE3MzE2NjQ0ODQsImlzcyI6IkJPT0tTVE9SRVdFQkFQSSIsImF1ZCI6IkJPT0tTVE9SRV9XRUJfQVBJIn0.DkOsD7lGpZihjLpzboGEYaRaPD5KGz4yCspV0AVgOmA"
//     });
    
// });

var jwtSection = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSection.GetValue<string>("Key"));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(
    options =>
    {
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSection.GetValue<string>("Issuer"),
                ValidAudience = jwtSection.GetValue<string>("Audience"),
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };

                    options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                var result = new { message = "unauthorization" };
                return context.Response.WriteAsJsonAsync(result);
            }
        };
        }
    }
);

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
app.MapGet("/", () => "Welcome to Book Store management Web API");

app.Run();
