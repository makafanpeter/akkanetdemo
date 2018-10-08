using System;
using System.Reflection;
using Akka.Actor;
using AkkaNetDemo.Baskets;
using AkkaNetDemo.Persistance;
using AkkaNetDemo.Products;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Swashbuckle.AspNetCore.Swagger;

namespace AkkaNetDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.LiterateConsole()
                .CreateLogger();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {

            var connectionString = Configuration["ConnectionString"];
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1).AddControllersAsServices();
            services.AddMvcCore()
                .AddJsonFormatters();


            
            services.AddEntityFrameworkSqlServer()
                .AddDbContext<ProductContext>(options =>
                    {
                        options.UseMySql(connectionString,
                            sqlOptions =>
                            {
                                sqlOptions.MigrationsAssembly(typeof(ProductContext).GetTypeInfo().Assembly.GetName().Name);
                                // ReSharper disable once ArgumentsStyleLiteral
                                // ReSharper disable once AssignNullToNotNullAttribute
                                sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                            });
                    } //Showing explicitly that the DbContext is shared across the HTTP request scope (graph of objects started in the HTTP request)

                );
            services.AddSingleton<ActorSystem>(_ => ActorSystem.Create("basketservice"));

            services.AddBasketServices();
            services.AddProductServices();

            services.AddSwaggerGen(options =>
            {

                options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("v1", new Info
                {
                    Title = "AKKA DEMO HTTP API",
                    Version = "v1",
                    Description = "The AKKA DEMO HTTP API",
                    TermsOfService = "Terms Of Service"
                });

                //var filePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, pathToDoc);
                //options.IncludeXmlComments(filePath);



               

            });

            var container = new ContainerBuilder();
            container.Populate(services);
            container.RegisterModule(new ApplicationModule(connectionString));
            return new AutofacServiceProvider(container.Build());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

           

            loggerFactory.AddSerilog();


            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

            });

            

            app.UseSwagger(c =>
            {

               c.RouteTemplate = "api/akka/docs/{documentName}/swagger.json";
                c.PreSerializeFilters.Add((swagger, httpReq) => swagger.Host = httpReq.Host.Value);
            });

            app.UseSwaggerUI(c =>
            {

                c.RoutePrefix = "api/akka/docs";
                c.SwaggerEndpoint("/api/akka/docs/v1/swagger.json", "V1");
            });
        }



        public class ApplicationModule
            : Autofac.Module
        {

            public string ConnectionString { get; }

            public ApplicationModule(string constr)
            {
                ConnectionString = constr;

            }

            protected override void Load(ContainerBuilder builder)
            {

                builder.Register(c => new DatabaseFactory(ConnectionString))
                    .As<IDatabaseFactory>()
                    .InstancePerLifetimeScope();
                var repositoriesAssembly = Assembly.GetAssembly(typeof(ProductRepository));
                builder.RegisterAssemblyTypes(repositoriesAssembly)
                    .Where(t => t.Name.EndsWith("Repository"))
                    .AsImplementedInterfaces();


                
            }
        }
    }
}
