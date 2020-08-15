// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.9.2

using System;
using System.Net.Http;
using Autofac;
using IntentRecognizer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.ML;
using ML.Bot.Bots;
using QnAProcessor;

namespace ML.Bot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson();

            // Create the Bot Framework Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            services.AddTransient<IBot, BotActivityHandler>();

            IIntentRecognizerFacade mlRecognizerFacade = (new IntentRecognizerFacade(new MLContext()));
            services.AddSingleton(mlRecognizerFacade);

            IQnAMachineLearningFacade mlQnaFacade = (new QnAMachineLearningFacade(new MLContext()));
            var qnAFileName = Configuration["QnAFile"];
            mlQnaFacade.Train(qnAFileName);
            services.AddSingleton(mlQnaFacade);

            IIntentRecognizerFacade mlIntentFacade = (new IntentRecognizerFacade(new MLContext()));
            var intentFileName = Configuration["IntentFile"];
            mlIntentFacade.Train(intentFileName);
            services.AddSingleton(mlIntentFacade);

            var storage = new MemoryStorage();
            // Create the Conversation state passing in the storage layer.
            var conversationState = new ConversationState(storage);
            services.AddSingleton(conversationState);

            //Open Weather Api Facade
            var httpClient = new HttpClient();
            var openWeatherApiKey = Configuration["OpenWeatherApiKey"];
            IOpenWeatherApiFacade openWeatherApiFacade = new OpenWeatherApiFacade(openWeatherApiKey, httpClient);
            services.AddSingleton(openWeatherApiFacade);
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            // Register your own things directly with Autofac, like:
            builder.RegisterType<WeatherIntentHandler>()
                .Named<IIntentHandler>("Weather")
                .SingleInstance();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseWebSockets()
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });

            // app.UseHttpsRedirection();
        }
    }

}
