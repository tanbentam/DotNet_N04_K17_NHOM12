using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelApp.Models
{
    public class FavoriteModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? HotelId { get; set; }
        public int? GuideId { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual UserModel User { get; set; }
        public virtual HotelModel Hotel { get; set; }
        public virtual UserModel Guide { get; set; }

        [NotMapped]
        public string TargetType =>
            HotelId.HasValue ? "Khách sạn" : "Hướng dẫn viên";

        [NotMapped]
        public string TargetName => Hotel?.Name ?? Guide?.FullName;
    }
}
