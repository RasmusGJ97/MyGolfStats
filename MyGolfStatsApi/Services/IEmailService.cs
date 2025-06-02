namespace MyGolfStatsApi.Services
{
    public interface IEmailService
    {
       Task RequestPasswordReset(string email);
       Task SendPasswordResetEmailAsync(string toEmail, string resetLink);
       Task ResetPassword(string token, string newPassword);
    }
}
