using System;
using System.ComponentModel.DataAnnotations;

namespace Library.WebApi.Models
{
    public class BookTrackingModel
    {
        public string UserId { get; set; }
        public int BookId { get; set; }
        public int BookTrackingId { get; set; }
        public string BookName { get; set; }
        public string UserName { get; set; }
        public int TotalQunatity { get; set; }
        public int ReserveQuantity { get; set; }
        public DateTime ReserveDate { get; set; }
        public DateTime ReturnDate { get; set; }
    }

    public class BookTrackingSaveModel
    {
        //public int Id { get; set; }
        [Required]
        public int BookId { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public int Qunatity { get; set; }
        public int BookingTypeId { get; set; }
        public DateTime ReserveDate { get; set; }
        public DateTime ReturnDate { get; set; }
    }
}
