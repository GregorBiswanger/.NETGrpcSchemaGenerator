using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetGrpcSchemaGenerator.Core;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DotNetGrpcSchemaGenerator
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });

            Task.Run(async () => 
            {
                if(await Electron.App.CommandLine.HasSwitchAsync("assembly-file") &&
                await Electron.App.CommandLine.HasSwitchAsync("target-path"))
                {
                    var assemblyFilePath = await Electron.App.CommandLine.GetSwitchValueAsync("assembly-file");
                    var destinationPath = await Electron.App.CommandLine.GetSwitchValueAsync("target-path");

                    var protoFileGenerator = new ProtoFileGenerator();
                    protoFileGenerator.SaveProtoFile(assemblyFilePath, destinationPath);

                    Console.WriteLine("Successfull generated!");
                    Electron.App.Exit();
                }
                else 
                {
                    var browserWindowOptions = new BrowserWindowOptions
                    {
                        WebPreferences = new WebPreferences
                        {
                            WebSecurity = false
                        }
                    };

                    await Electron.WindowManager.CreateWindowAsync(browserWindowOptions);
                }
            });
        }
    }
}
