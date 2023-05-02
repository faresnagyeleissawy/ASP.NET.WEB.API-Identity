using EmployeeIdentity.APIs.Data.Context;
using EmployeeIdentity.APIs.Data.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllawCall", options =>
    {
        options.AllowAnyHeader();
        options.AllowAnyOrigin();
        options.AllowAnyMethod();


    });
});
#endregion
#region database
string? conString = builder.Configuration.GetConnectionString ("connection")!;
builder.Services.AddDbContext<DepartmentContext>(options =>
options.UseNpgsql(conString)
);



#endregion
#region usermanager
builder.Services.AddIdentity<Employee, IdentityRole>(options =>
{ options.Password.RequireNonAlphanumeric = false;
  options.Password.RequireUppercase=false;

}).AddEntityFrameworkStores<DepartmentContext>();
#endregion
#region Authorazation
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ManagersOnly", policy =>
            policy.RequireClaim(ClaimTypes.Role, "Manager", "CEO")
            .RequireClaim(ClaimTypes.NameIdentifier));
    options.AddPolicy("Employee", policy =>
               policy.RequireClaim(ClaimTypes.Role, "Employee", "Manager", "CEO")
               .RequireClaim(ClaimTypes.NameIdentifier));



});

#endregion
#region Authentication 
string? secretKey = builder.Configuration.GetValue<string>("SecretKey")!;
var keyByteArray = Encoding.ASCII.GetBytes(secretKey);
var key = new SymmetricSecurityKey(keyByteArray);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "scema";
    options.DefaultChallengeScheme = "scema";
}).AddJwtBearer("scema", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        IssuerSigningKey = key,
        ValidateAudience = false,   

    };
});

#endregion
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseCors("AllawCall");
app.UseAuthentication();
app.MapControllers();

app.Run();
