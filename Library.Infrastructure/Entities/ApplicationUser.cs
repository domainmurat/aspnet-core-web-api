using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Infrastructure.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            BookTrackings = new HashSet<BookTracking>();
        }
        public virtual ICollection<BookTracking> BookTrackings { get; set; }
    }
}
