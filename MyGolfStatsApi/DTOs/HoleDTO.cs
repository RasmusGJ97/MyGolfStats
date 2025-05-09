using MyGolfStatsApi.Db.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyGolfStatsApi.DTOs
{
    public class HoleDTO
    {
        public int HoleNumber { get; set; }
        public int Par { get; set; }
    }
}
