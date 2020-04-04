using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using BooksMiddletier.OtherClasses;
using Microsoft.Owin;
using Owin;
using Serilog;
using Serilog.Sinks.Email;

[assembly: OwinStartup(typeof(BooksApi.Startup))]

namespace BooksApi
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.MapSignalR();
            //var log = new LoggerConfiguration().WriteTo.Email(fromEmail: "no-reply@mobxsoft.com", toEmail: "maioralin@gmail.com", mailServer: "smtp.zoho.com", networkCredential: credential).CreateLogger();
            var log = new LoggerConfiguration().WriteTo.EmailSink().WriteTo.File("C:\\Work\\logs\\log.txt").CreateLogger();
            GlobalVars.Logger = log;
        }
    }
}
