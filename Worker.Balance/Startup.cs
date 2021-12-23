using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Worker.Balance.Data;
using Worker.Balance.Repositories;
using Worker.Balance.Services;
using Microsoft.EntityFrameworkCore;
using Confluent.Kafka;
using EventSourcing.Shared.Settings;
using Worker.Balance.Handlers;

namespace Worker.Balance
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
            services.Configure<KafkaProducerSettings>(Configuration.GetSection("KafkaProducerSettings"));
            services.Configure<KafkaConsumerSettings>(Configuration.GetSection("KafkaConsumerSettings"));

            services.AddDbContext<BalanceContext>(
                options => options.UseMySql(Configuration.GetConnectionString("DefaultConnection")),
                contextLifetime: ServiceLifetime.Transient,
                optionsLifetime: ServiceLifetime.Singleton
            );
            services.AddControllers();
            services.AddTransient<IAccountBalanceService, AccountBalanceService>();
            services.AddTransient<IBalanceDbRepository, BalanceDbRepository>();
            services.AddTransient<IBalanceQueueRepository, BalanceQueueRepository>();
            services.AddTransient<ICommandHandler, CommandHandler>();
            services.AddTransient<IEventHandler, Worker.Balance.Handlers.EventHandler>();
            services.AddTransient<IQueryHandler, QueryHandler>();
            services.AddHostedService<BalanceQueueService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            UpdateDatabase(app);

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private static void UpdateDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<BalanceContext>())
                {
                    context.Database.Migrate();
                }
            }

        }
    }
}
