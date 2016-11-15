using AutoMapper;
using FluentValidation.AspNetCore;
using Framework.AspNetCore.Filters;
using Framework.AspNetCore.HtmlTags;
using Framework.AspNetCore.ModelBinding;
using Framework.AspNetCore.Razor;
using HtmlTags;
using HtmlTags.Conventions.Elements;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Framework.AspNetCore
{
    public abstract class Bootstrap
    {
        public IConfigurationRoot Configuration { get; }

        public Bootstrap(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            var mvcBuilder = services.AddMvc(opt =>
            {
                opt.Conventions.Add(new FeatureConvention());
                opt.Filters.Add(typeof(TransactionContextFilter));
                opt.Filters.Add(typeof(ValidatorActionFilter));

                if (EntityBaseType != null && EntityModelBinder != null)
                {
                    opt.ModelBinderProviders.Insert(0, new EntityModelBinderProvider(EntityBaseType, EntityModelBinder));
                }
            })
            .AddRazorOptions(opt =>
            {
                // {0} - Action Name
                // {1} - Controller Name
                // {2} - Area Name
                // {3} - Feature Name
                // Replace normal view location entirely
                opt.ViewLocationFormats.Clear();
                opt.ViewLocationFormats.Add("/Features/{3}/{1}/{0}.cshtml");
                opt.ViewLocationFormats.Add("/Features/{3}/{0}.cshtml");
                opt.ViewLocationFormats.Add("/Features/Shared/{0}.cshtml");
                opt.ViewLocationExpanders.Add(new FeatureViewLocationExpander());
            });

            if (ValidatorAssemblyMarkers != null)
            {
                mvcBuilder.AddFluentValidation(cfg =>
                {
                    foreach(var marker in ValidatorAssemblyMarkers)
                    {
                        cfg.RegisterValidatorsFromAssemblyContaining(marker);
                    }
                });
            }

            if (MapperProfileAssemblyMarkers != null)
            {
                services.AddAutoMapper(MapperProfileAssemblyMarkers);
                Mapper.AssertConfigurationIsValid();
            }

            if (MediatRAssemblyMarkers != null)
            {
                services.AddMediatR(MediatRAssemblyMarkers);
            }

            ConfigureDbContext(services);

            services.AddHtmlTags(new TagConventions(TagBuilderPolicies));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });
        }

        protected virtual string ErrorPath => "/Home/Error";

        protected virtual Type[] MapperProfileAssemblyMarkers => null;

        protected virtual Type[] ValidatorAssemblyMarkers => null;

        protected virtual Type[] MediatRAssemblyMarkers => null;

        protected virtual Type EntityBaseType => null;

        protected virtual EntityModelBinder EntityModelBinder => null;

        protected virtual string ConnectionString => Configuration["Data:DefaultConnection:ConnectionString"];

        protected virtual IElementBuilderPolicy[] TagBuilderPolicies => Array.Empty<IElementBuilderPolicy>();

        protected abstract void ConfigureRoutes(IRouteBuilder routes);

        // services.AddScoped(_ => new MyDBContext(ConnectionString));
        // services.AddScoped<ITransactionContext, MyDBContext>();
        protected abstract void ConfigureDbContext(IServiceCollection services);
    }
}
