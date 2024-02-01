
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using SFA.DAS.QnA.Api.Authentication;
using SFA.DAS.QnA.Api.Authorization;
using SFA.DAS.QnA.Api.Infrastructure;
using SFA.DAS.QnA.Application;
using SFA.DAS.QnA.Application.Commands;
using SFA.DAS.QnA.Application.Commands.Files;
using SFA.DAS.QnA.Application.Commands.StartApplication;
using SFA.DAS.QnA.Application.Services;
using SFA.DAS.QnA.Application.Validators;
using SFA.DAS.QnA.Configuration.Config;
using SFA.DAS.QnA.Configuration.Infrastructure;
using SFA.DAS.QnA.Data;


namespace SFA.DAS.QnA.Api
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            Configuration = configuration;

            var config = new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .AddAzureTableStorageConfiguration(
                    configuration["ConfigurationStorageConnectionString"],
                    configuration["ConfigNames"],
                    configuration["Environment"],
                    configuration["Version"]
                ).Build();

            Configuration = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<QnAConfig>(Configuration.GetSection("QnA"));
            services.Configure<AzureActiveDirectoryConfiguration>(Configuration.GetSection("AzureActiveDirectoryConfiguration"));
            services.Configure<FileStorageConfig>(Configuration.GetSection("FileStorage"));
            var serviceProvider = services.BuildServiceProvider();
            var config = serviceProvider.GetService<IOptions<QnAConfig>>();
            IdentityModelEventSource.ShowPII = false;

            services.AddApiAuthorization(_hostingEnvironment);
            services.AddApiAuthentication(serviceProvider);

            services.RegisterAllTypes<IValidator>(new[] { typeof(IValidator).Assembly });
            services.AddTransient<IValidatorFactory, ValidatorFactory>();
            services.AddTransient<IAnswerValidator, AnswerValidator>();
            services.AddTransient<IFileContentValidator, FileContentValidator>();
            services.AddTransient<IApplicationDataValidator, ApplicationDataValidator>();

            services.AddTransient<IEncryptionService, EncryptionService>();
            services.AddTransient<INotRequiredProcessor, NotRequiredProcessor>();
            services.AddTransient<IKeyProvider, ConfigKeyProvider>();
            services.AddTransient<ITagProcessingService, TagProcessingService>();
            services.AddAutoMapper(typeof(SystemTime).Assembly);
            services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());

            services.AddDbContext<QnaDataContext>(options =>
            {
                var qnaSqlConnectionString = config.Value.SqlConnectionstring;

                var connection = new System.Data.SqlClient.SqlConnection(qnaSqlConnectionString);

                if (!_hostingEnvironment.IsDevelopment())
                {
                    var generateTokenTask = SqlTokenGenerator.GenerateTokenAsync();
                    connection.AccessToken = generateTokenTask.GetAwaiter().GetResult();
                }

                options.UseSqlServer(connection, providerOptions => providerOptions.EnableRetryOnFailure());
            });

            services.AddEntityFrameworkSqlServer();

            services.AddMvc(setup =>
            {
                setup.EnableEndpointRouting = false;
                if (!_hostingEnvironment.IsDevelopment())
                {
                    setup.Filters.Add(new AuthorizeFilter("default"));
                }
                setup.Conventions.Add(new ApiExplorerGroupConvention());
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "QnA API", Version = "0.1" });
                c.SwaggerDoc("config", new OpenApiInfo { Title = "QnA API Config", Version = "0.1" });

                if (_hostingEnvironment.IsDevelopment())
                {
                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    c.IncludeXmlComments(xmlPath);
                }
            });

            services.AddHealthChecks().AddDbContextCheck<QnaDataContext>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                app.UseAuthentication();
            }

            app.UseExceptionHandler("/errors/500");
            app.UseStatusCodePagesWithReExecute("/errors/{0}");

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "QnA API");
                c.SwaggerEndpoint("/swagger/config/swagger.json", "QnA API Config");
            });

            app.UseHealthChecks("/health");
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "api/{controller}/{action}/{id?}");
            });
        }
    }
}
