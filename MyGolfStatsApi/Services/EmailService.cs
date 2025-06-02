using Microsoft.EntityFrameworkCore;
using MyGolfStatsApi.Db.AppDbContext;
using MyGolfStatsApi.Db.Models;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace MyGolfStatsApi.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly AppDbContext _context;

        public EmailService(IConfiguration config, AppDbContext dbContext)
        {
            _config = config;
            _context = dbContext;
        }

        public async Task RequestPasswordReset(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                throw new Exception("Ingen användare med den e-postadressen.");

            var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            var resetToken = new PasswordResetToken
            {
                UserId = user.Id,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };

            _context.PasswordResetTokens.Add(resetToken);
            await _context.SaveChangesAsync();

            var frontendUrl = _config["Client:ProdBaseUrl"];
            var resetLink = $"{frontendUrl}/reset-password?token={HttpUtility.UrlEncode(token)}";


            await SendPasswordResetEmailAsync(user.Email, resetLink);
        }


        public async Task SendPasswordResetEmailAsync(string toEmail, string resetLink)
        {
            var smtpSection = _config.GetSection("Smtp");
            var fromEmail = smtpSection["Username"];

            var message = new MailMessage
            {
                From = new MailAddress(fromEmail, "MyGolfStats"),
                Subject = "Återställ ditt lösenord",
                Body = $"<p>Klicka på länken för att återställa ditt lösenord:</p><a href='{resetLink}'>{resetLink}</a>",
                IsBodyHtml = true
            };
            message.To.Add(toEmail);

            using var smtp = new SmtpClient
            {
                Host = smtpSection["Host"],
                Port = int.Parse(smtpSection["Port"]),
                EnableSsl = bool.Parse(smtpSection["EnableSsl"]),
                Credentials = new NetworkCredential(fromEmail, smtpSection["Password"])
            };

            await smtp.SendMailAsync(message);
        }

        public async Task ResetPassword(string token, string newPassword)
        {
            var resetToken = await _context.PasswordResetTokens
                .FirstOrDefaultAsync(t => t.Token == token && !t.Used && t.ExpiresAt > DateTime.UtcNow);

            if (resetToken == null)
                throw new Exception("Token ogiltig eller utgången.");

            var user = await _context.Users.FindAsync(resetToken.UserId);
            if (user == null)
                throw new Exception("Användare hittades inte.");

            user.PasswordHash = PasswordHasher.Hash(newPassword);
            resetToken.Used = true;

            _context.Users.Update(user);
            _context.PasswordResetTokens.Update(resetToken);
            await _context.SaveChangesAsync();
        }
    }
}
