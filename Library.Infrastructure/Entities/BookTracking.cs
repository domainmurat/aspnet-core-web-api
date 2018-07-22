using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Infrastructure.Entities
{
    [Table("BookTracking")]
    public class BookTracking
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string UserId { get; set; }
        public int Qunatity { get; set; }
        public int BookingTypeId { get; set; }
        public DateTime ReserveDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public DateTime ActualReturnDate { get; set; }
        public virtual Book Book { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
