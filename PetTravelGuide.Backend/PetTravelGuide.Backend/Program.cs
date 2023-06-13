using System.IdentityModel.Tokens.Jwt;
using System.Text;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PetTravelGuide.Backend.Database;
using PetTravelGuide.Backend.Models.AppSettings;
using PetTravelGuide.Backend.Services.LocationService;
using PetTravelGuide.Backend.Services.MapService;
using PetTravelGuide.Backend.Services.PetService;
using PetTravelGuide.Backend.Services.UserService;
using PetTravelGuide.Backend.Utils;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
{
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SigningKey"])),
    RequireSignedTokens = true,
    // Ensure the token hasn't expired
    RequireExpirationTime = true,
    ValidateLifetime = true,
    // Ensure the token audience matches our audience value
    ValidateAudience = true,
    ValidAudience = builder.Configuration["JwtSettings:Audience"],
    // Ensure the token was issued by a trusted authorization server
    ValidateIssuer = true,
    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
    // Clock skew compensates for server time drift
    ClockSkew = TimeSpan.Zero, // Zero because on same server
    // Specify the custom role claim type
    RoleClaimType = JwtTokenBuilder.Role
};


// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", corsBuilder => 
        corsBuilder.AllowAnyOrigin().AllowAnyHeader().WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS"));
});
builder.Services.Configure<JwtAuthenticationSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<UserServiceSettings>(builder.Configuration.GetSection("UserServiceSettings"));
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(configureOptions =>
{
    configureOptions.TokenValidationParameters = tokenValidationParameters;
});
builder.Services.AddAuthorization();
// Mapping database
builder.Services.AddDbContext<DatabaseContext>((_, optionsBuilder) =>
{
    optionsBuilder.UseNpgsql(builder.Configuration["PostgreSql:ConnectionString"]);
});
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddControllers()            
    .AddNewtonsoftJson(jsonOptions =>
    {
        jsonOptions.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        jsonOptions.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); 
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IPetService, PetService>();
builder.Services.AddScoped<IMapService, MapService>();

var app = builder.Build();

app.UseRouting();
app.UseCors("CorsPolicy");

app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
        
app.UseAuthorization();

app.MapControllers();

app.Run();