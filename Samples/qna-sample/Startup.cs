using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.BotFramework;
using Microsoft.Bot.Builder.Integration;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

// For use of DialogGen library
using DialogGen.Lib;
using System.IO;

namespace qna_sample
{
    public class Startup
    {
        private IConfiguration configuration;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();
            configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddBot<MyBot>(options =>
            {
                // Create a storage to persist our values
                IStorage dataStore = new MemoryStorage();

                // Store ConverstationState (for current conversation) and UserState (for tracking user interaction)
                var conversationState = new ConversationState(dataStore);
                var userState = new UserState(dataStore);
                
                options.CredentialProvider = new ConfigurationCredentialProvider(configuration);               
                
                // Add the State to the bot
                options.State.Add(conversationState);
                options.State.Add(userState);
            });

            DialogGenerator dialogGenerator = new DialogGenerator();

            dialogGenerator.InitializeDialogGenerator(services, Path.GetFullPath("generator/qnaDialog.json"));
        }

        

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseBotFramework();
        }
    }
}
