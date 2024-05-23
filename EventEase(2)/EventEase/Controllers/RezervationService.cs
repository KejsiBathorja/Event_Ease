using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using EventEase.Models;
using Microsoft.AspNetCore.Authorization;

namespace EventEase.Controllers
{
    [Authorize(Roles = "User")]
    public class RezervationService : Controller
    {
        private readonly string smtpServer = "smtp.gmail.com";
        private readonly int smtpPort = 465;
        private readonly string smtpUsername = "bathorjakejsi@gmail.com";
        private readonly string smtpPassword = "kkejjssii33333";

        public async Task<bool> ReserveTicket(Event @event, string userEmail, int numTickets, decimal totalAmount)
        {
            try
            {
                // Kryej procesin e rezervimit të biletës
                // ...

                // Dërgo email për konfirmim tek përdoruesi
                await SendConfirmationEmail(userEmail, @event.Name, totalAmount);

                // Dërgo email njoftim tek organizatori i eventit
                await SendNotificationEmail(@event.Organizer.Email, @event.Name, numTickets);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Gabim gjatë rezervimit të biletës: {ex.Message}");
                return false;
            }
        }

        private async Task SendConfirmationEmail(string toEmail, string eventName, decimal totalAmount)
        {
            using (var client = new SmtpClient("smtp.gmail.com", 465))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                client.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpUsername),
                    Subject = "Rezervimi i biletës u krye me sukses!",
                    Body = $"Ju keni rezervuar një biletë për eventin {eventName}. Shihemi në event!",
                    IsBodyHtml = false
                };

                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
            }
        }

        private async Task SendNotificationEmail(string toEmail, string eventName, int numTickets)
        {
            using (var client = new SmtpClient("smtp.gmail.com", 465))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                client.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpUsername),
                    Subject = "Rezervim i ri biletash!",
                    Body = $"Një person ka rezervuar një biletë për eventin {eventName}. Ju lutemi kontrolloni rezervimet tuaja.",
                    IsBodyHtml = false
                };

                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
            }
        }
    }
}
