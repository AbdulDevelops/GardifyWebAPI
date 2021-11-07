
using GardifyNewsletter.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace GardifyNewsletter.Services
{
    public class EmailQueueService : IHostedService, IDisposable
    {
        private int executionCount = 0;
        private readonly ILogger<EmailQueueService> _logger;
        private Timer _timer;
        private IServiceProvider Services;

        public EmailQueueService(IServiceProvider services, ILogger<EmailQueueService> logger)
        {
            Services = services;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromHours(1));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {

            using (var scope = Services.CreateScope())
            {
                var _context =
                    scope.ServiceProvider
                        .GetRequiredService<ApplicationDbContext>();

                var _env =
                    scope.ServiceProvider
                        .GetRequiredService<Microsoft.AspNetCore.Hosting.IHostingEnvironment>();

                try
                {


                    File.AppendAllText(_env.WebRootPath + "log_email.txt", $"{DateTime.Now}: dowork() started\n");

                    var count = Interlocked.Increment(ref executionCount);

                    _logger.LogInformation(
                        "Timed Hosted Service is working. Count: {Count}", count);

                    var spoolEntries = _context.NewsletterSpool.Take(Program.EMAIL_AMOUNT).ToList();

                    File.AppendAllText(_env.WebRootPath + "log_email.txt", $"{DateTime.Now}: found {spoolEntries.Count} spoolentries\n");


                    //Send out all active entries
                    foreach (var spoolEntry in spoolEntries)
                    {
                        try
                        {

                            string bodyModified;

                            string newsletterUnsubscribe = "https://gardifybackend.sslbeta.de/api/newsletterapi/unsubscribe/";
                            bodyModified = spoolEntry.Body.Replace("%GUID%", newsletterUnsubscribe + "" + spoolEntry.UserId.ToString());

                            if (String.IsNullOrEmpty(bodyModified))
                                bodyModified = null;

                            SendEmailAsync(_env, spoolEntry.FromEmail,
                                spoolEntry.FromName,
                                spoolEntry.RecipientEmail,
                                spoolEntry.FromReplyTo,
                                spoolEntry.Subject,
                                bodyModified).Wait();

                        }
                        catch (Exception e)
                        {
                            File.AppendAllText(_env.WebRootPath + "log_email.txt", $"{DateTime.Now}: Exception in Spool Entry: {spoolEntry.NewsletterSpoolId} \n");
                        }

                    }

                    File.AppendAllText(_env.WebRootPath + "log_email.txt", $"{DateTime.Now}: finished sending\n");

                    //Write entries into SpoolArchive
                    var archivedEntries = spoolEntries.Select(s => new NewsletterSpoolArchive
                    {
                        AddedToSpool = s.AddedToSpool,
                        //Body = s.Body, //Body should stay empty to save space?
                        Credentials = s.Credentials,
                        FromEmail = s.FromEmail,
                        FromName = s.FromName,
                        FromReplyTo = s.FromReplyTo,
                        Html = s.Html,
                        NewsletterDistributionListId = s.NewsletterDistributionListId,
                        NewsletterId = s.NewsletterId,
                        Port = s.Port,
                        RecipientEmail = s.RecipientEmail,
                        RecipientId = s.RecipientId,
                        Scheduled = s.Scheduled,
                        Send = s.Send,
                        SendedDate = DateTime.Now,
                        SenderDomain = s.SenderDomain,
                        Subject = s.Subject,
                        UserId = s.UserId
                    });
                    _context.NewsletterSpoolArchive.AddRange(archivedEntries);

                    //Remove entries from spool
                    _context.RemoveRange(spoolEntries);
                    _context.SaveChanges();

                    File.AppendAllText(_env.WebRootPath + "log_email.txt", $"{DateTime.Now}: added to archive and removed\n");

                }
                catch (Exception e)
                {
                    File.AppendAllText(_env.WebRootPath + "log_email.txt", $"{DateTime.Now}: exception occured\n {e}\n");
                }
            }



        }

        public async Task SendEmailAsync(Microsoft.AspNetCore.Hosting.IHostingEnvironment _env, string fromEmail, string fromName, string to, string replyTo, string subject, string message, IDictionary<string, string> files = null, bool storeLocally = false)
        {
            // if replyTo empty then set fromEmail to replyTo
            if (replyTo == null)
            {
                replyTo = fromEmail;
            }
            if (String.IsNullOrEmpty(message))
                throw new Exception("body is null");
            try
            {
                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                };

                mail.ReplyToList.Add(replyTo);
                mail.To.Add(new MailAddress(to));

                if (files != null)
                {
                    foreach (var file in files)
                    {
                        var fileName = Path.Combine(_env.WebRootPath, "files", file.Key);
                        Attachment at = new Attachment(fileName)
                        {
                            Name = file.Value
                        };
                        at.ContentDisposition.FileName = file.Value;
                        mail.Attachments.Add(at);
                    }
                }

                using (SmtpClient smtp = new SmtpClient())
                {
                    if (!storeLocally)
                    {
                        smtp.EnableSsl = true;
                        smtp.Host = "mail.netzlab14.de";
                        smtp.Port = 587;
                        smtp.Credentials = new NetworkCredential("web@mail.netzlab14.de", "netzLab14!");
                    }
                    else
                    {
                        smtp.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                        smtp.PickupDirectoryLocation = "C:\\EMail";
                    }
                    await smtp.SendMailAsync(mail);
                }
            }
            catch
            {
                await Task.CompletedTask;
                throw;
            }
        }


        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
