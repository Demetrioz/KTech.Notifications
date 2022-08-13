using Akka.Actor;
using Akka.Actor.Setup;
using Akka.Configuration;
using Akka.DependencyInjection;
using Hangfire;
using Hangfire.PostgreSql;
using KTech.Notifications.Core.Actors;
using KTech.Notifications.Core.Config;
using KTech.Notifications.Core.Services.EmailService;
using KTech.Notifications.Core.Services.SmsService;
using SendGrid.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

//////////////////////////////////////////
//          Configure Settings          //
//////////////////////////////////////////

CoreSettings settings = new();
builder.Configuration.GetSection("AppSettings").Bind(settings);

builder.Services.Configure<TwilioSettings>(
    builder.Configuration.GetSection("AppSettings:Twilio"));
builder.Services.Configure<RabbitMqSettings>(
    builder.Configuration.GetSection("AppSettings:RabbitMQ"));

//////////////////////////////////////////
//               Hangfire               //
//////////////////////////////////////////

builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(builder.Configuration.GetConnectionString("NotificationDb")));

builder.Services.AddHangfireServer();

//////////////////////////////////////////
//         Notification Services        //
//////////////////////////////////////////

builder.Services.AddSendGrid(options =>
{
    options.ApiKey = settings.SendGrid.Key;
});

builder.Services.AddTransient<IEmailService, SendGridService>();
builder.Services.AddHttpClient<ISmsService, TwilioService>();

//////////////////////////////////////////
//               Akka.Net               //
//////////////////////////////////////////

builder.Services.AddSingleton(provider =>
{
    Config hocon = ConfigurationFactory.Default();
    BootstrapSetup bootStrap = BootstrapSetup.Create().WithConfig(hocon);
    DependencyResolverSetup di = DependencyResolverSetup.Create(provider);
    ActorSystemSetup actorSystemSetup = bootStrap.And(di);

    // Create the actor system
    ActorSystem system = ActorSystem.Create("NotificationsCore", actorSystemSetup);

    Props managerProps = DependencyResolver
        .For(system).Props<NotificationManager>();

    // Create the manager
    system.ActorOf(managerProps, "NotificationManager");

    return system;
});

//////////////////////////////////////////
//            MVC Components            //
//////////////////////////////////////////

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//////////////////////////////////////////
//    Build and Run Startup Actions     //
//////////////////////////////////////////

var app = builder.Build();

app.Lifetime.ApplicationStarted.Register(() =>
{
    // Start Akka.net
    app.Services.GetRequiredService<ActorSystem>();
});

app.Lifetime.ApplicationStopped.Register(() =>
{
    // Stop Akka.net
    ActorSystem system = app.Services.GetRequiredService<ActorSystem>();
    CoordinatedShutdown.Get(system).Run(CoordinatedShutdown.ClrExitReason.Instance).Wait();
});

//////////////////////////////////////////
//      Configure Request Pipeline      //
//////////////////////////////////////////

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// TODO: Configure dashboard authentication
// https://docs.hangfire.io/en/latest/configuration/using-dashboard.html#configuring-authorization
app.UseHangfireDashboard();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
