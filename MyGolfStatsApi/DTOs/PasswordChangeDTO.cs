﻿namespace MyGolfStatsApi.DTOs
{
    public class PasswordChangeDTO
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public Guid UserId { get; set; }    
    }
}
