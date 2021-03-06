﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reminder.Storage.Core;
using Reminder.Storage.InMemory;
using Reminder.Storage.DbStorage;
using Reminder.Storage.SqlServer.ADO;
using System;

namespace Reminder.Storage.WebApi
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
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            //services.AddSingleton<IReminderStorage>(new InMemoryReminderStorage());
            //services.AddSingleton<IReminderStorage>(new DataBaseReminderStorage());

            string connectionString = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build()
                .GetConnectionString("MyTestConnection");

            services.AddSingleton<IReminderStorage>(new DataBaseReminderStorage(connectionString));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseMvc();
		}
	}
}
