using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.File;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace BooksApi
{
    public class SerilogEmail : ILogEventSink
    {
        private readonly IFormatProvider _formatProvider;
        private CloudBlobContainer _cloudBlobContainer;

        public SerilogEmail(IFormatProvider formatProvider)
        {
            _formatProvider = formatProvider;
            string storageConnectionString = "";
            CloudStorageAccount account = CloudStorageAccount.Parse(storageConnectionString);
            CloudBlobClient cloudBlobClient = account.CreateCloudBlobClient();
            CloudBlobContainer container = cloudBlobClient.GetContainerReference("logs");
            container.CreateIfNotExistsAsync();
            _cloudBlobContainer = container;
        }

        public void Emit(LogEvent logEvent)
        {
            var message = logEvent.RenderMessage(_formatProvider);
            CloudAppendBlob appBlob = _cloudBlobContainer.GetAppendBlobReference(string.Format($"BooksAPILogs{DateTime.Now.ToString("ddMMyyy")}.txt"));
            if (!appBlob.Exists())
            {
                appBlob.CreateOrReplace();
            }

            appBlob.AppendText($"[{logEvent.Timestamp}] - { logEvent.Level} - {message}\r\n");
            string exceptionMessage = string.Empty;
            if(logEvent.Exception != null)
            {
                exceptionMessage = logEvent.Exception.ToString();
                appBlob.AppendText($"{exceptionMessage}\r\n");
            }
            if (logEvent.Level == LogEventLevel.Error || logEvent.Level == LogEventLevel.Fatal)
            {
                using (SmtpClient smtp = new SmtpClient("smtp.zoho.com", 587))
                {
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential("noreply@mobxsoft.com", "");
                    using (MailMessage msg = new MailMessage("noreply@mobxsoft.com", ""))
                    {
                        msg.Subject = message;
                        msg.Body = $"[{ logEvent.Timestamp}] - { logEvent.Level} - { message}\r\n{ exceptionMessage }";

                        smtp.Send(msg);
                    }
                }
            }
        }
    }

    public static class MySinkExtensions
    {
        public static LoggerConfiguration EmailSink(
                  this LoggerSinkConfiguration loggerConfiguration,
                  IFormatProvider formatProvider = null)
        {
            return loggerConfiguration.Sink(new SerilogEmail(formatProvider));
        }
    }
}