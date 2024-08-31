using HealthChecks.UI.Client;
using Hrms.Model;
using Hrms.Provider;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OfficeOpenXml;
using System.Security.Claims;
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSwaggerGen(s => { s.AddSecurityDefinition("Bearer", new() { Name = "Authorization", In = ParameterLocation.Header, Type = SecuritySchemeType.ApiKey, Description = "Enter JWT with Bearer into field" }); s.AddSecurityRequirement(new OpenApiSecurityRequirement { { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, Array.Empty<string>() } }); });
builder.Services.AddAuthentication(a => { a.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; a.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; a.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; }).AddJwtBearer(a => { a.RequireHttpsMetadata = false; a.TokenValidationParameters = new() { IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(AccessProvider.EncryptionKey)), ValidateIssuer = false, ValidateLifetime = true, ClockSkew = TimeSpan.Zero, ValidateAudience = false }; });
builder.Services.AddScoped(p => { Claim claim = ((ClaimsIdentity)p.GetService<IHttpContextAccessor>().HttpContext.User.Identity).Claims.FirstOrDefault(c => c.Type == ClaimTypes.UserData); return claim != null && !string.IsNullOrWhiteSpace(claim.Value) ? claim.Value.FromJson<Employee>() : null; });
builder.Services.AddCors();

WebApplication app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHealthChecks("/", new HealthCheckOptions() { Predicate = _ => true, ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse });
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors(o => o.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.MapControllers();
app.UseWebSockets(new WebSocketOptions() { KeepAliveInterval = TimeSpan.FromSeconds(30) });
app.Run();
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;