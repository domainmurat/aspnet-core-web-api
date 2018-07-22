using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Library.Infrastructure.Entities
{
    [Table("Book")]
    public class Book
    {
        public Book()
        {
            BookTrackings = new HashSet<BookTracking>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public virtual ICollection<BookTracking> BookTrackings { get; set; }
    }
}
