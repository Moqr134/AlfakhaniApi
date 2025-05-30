using App.ISirvice;
using App.Service;
using Infrastructure.ORM;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DBContext>(option => option.UseSqlServer(DBConn.ConnectionString));

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;

                    byte[] symmetrickey = Convert.FromBase64String(DBConn.SecretKey);
                    SymmetricSecurityKey securityKey = new SymmetricSecurityKey(symmetrickey);

                    cfg.TokenValidationParameters = new TokenValidationParameters()
                    {
                        IssuerSigningKey = securityKey,
                        ValidIssuer = "MohamadProject",
                        ValidAudience = "Users",
                        ValidateLifetime = true,
                        RequireExpirationTime = true,
                        ClockSkew = TimeSpan.FromDays(10)
                    };
                });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseAuthentication();

app.MapControllers();

app.Run();
