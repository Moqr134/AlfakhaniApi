using App.ISirvice;
using App.Service;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Infrastructure.ORM;
using Infrastructure.Service;
using Infrastructure.Twilio;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SecondApi.App.ISirvice;
using SecondApi.App.Service;
using SecondApi.Domin.Item;
using SecondApi.Infrastructure.Notifications;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache();

builder.Services.AddDbContext<DBContext>(option => option.UseSqlServer(DBConn.ConnectionString));

builder.Services.Register<IScopped>();

builder.Services.Register<ISingleton>();

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IItemService, ItemService>();

builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddScoped<IBillsService, BillsService>();

builder.Services.AddScoped<ITwilio, TwilioService>();

builder.Services.AddScoped<INotifService, NotifService>();

builder.Services.AddScoped<ISizesService, SizesService>();

builder.Services.AddScoped<IFcmService, FcmService>();

builder.Services.AddSingleton<ItemCash>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorCors", policy =>
    {
        policy
            .WithOrigins(
                "https://www.alfakhani.com",
                "https://alfakhani.com"
            )
            //.WithOrigins("https://localhost:7126")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

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
                        ValidIssuer = "PizzaProject",
                        ValidAudience = "Users",
                        ValidateLifetime = true,
                        RequireExpirationTime = true,
                        ClockSkew = TimeSpan.FromDays(10)
                    };
                    cfg.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Cookies["AuthToken"];
                            if (!string.IsNullOrEmpty(accessToken))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

builder.Services.AddSignalR();

builder.Services.AddScoped<INotificationService, NotificationService>();

FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Infrastructure\\Notifications\\alfkhani-e0981-firebase-adminsdk-fbsvc-337b6114e4.json")),
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.UseCors("BlazorCors");

app.UseStaticFiles();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHub<NotificationHub>("notificationhub");

app.Run();
