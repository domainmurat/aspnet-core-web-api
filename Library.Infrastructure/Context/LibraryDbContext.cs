using Library.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Infrastructure.Context
{
    public class LibraryDbContext : IdentityDbContext<ApplicationUser>
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Book { get; set; }
        public DbSet<BookTracking> BookTracking { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);

            builder.Entity<BookTracking>().HasOne(p => p.Book)
                .WithMany(x => x.BookTrackings)
                .HasForeignKey(x => x.BookId);

            builder.Entity<BookTracking>().HasOne(p => p.ApplicationUser)
            .WithMany(x => x.BookTrackings)
            .HasForeignKey(x => x.UserId);
        }
    }
}
