using EventSourcing.Shared.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Worker.Transaction.Data;
using Worker.Transaction.Handlers;
using Worker.Transaction.Repositories;
using Worker.Transaction.Services;

namespace Worker.Transaction
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
            services.Configure<KafkaConsumerSettings>(Configuration.GetSection("KafkaConsumerSettings"));
            services.AddDbContext<TransactionContext>(
                options => options.UseMySql(Configuration.GetConnectionString("DefaultConnection")),
                contextLifetime: ServiceLifetime.Transient,
                optionsLifetime: ServiceLifetime.Singleton
            );

            services.AddControllers();

            services.AddTransient<IMoneyTransactionService, MoneyTransactionService>();
            services.AddTransient<ITransactionDbRepository, TransactionDbRepository>();
            services.AddTransient<IEventHandler, Worker.Transaction.Handlers.EventHandler>();
            services.AddHostedService<TransactionQueueService>();
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
                using (var context = serviceScope.ServiceProvider.GetService<TransactionContext>())
                {
                    context.Database.Migrate();
                }
            }
        }
    }
}
