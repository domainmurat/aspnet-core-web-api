using Library.Infrastructure.Constant;
using Library.Infrastructure.Context;
using Library.Infrastructure.Entities;
using Library.Infrastructure.Interfaces;
using Library.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Library.Infrastructure.Repository
{
    public class BookTrackingRepository : EfRepository<BookTracking>, IBookTrackingRepository
    {
        private readonly IRepository<Book> _bookRepository;
        public BookTrackingRepository(LibraryDbContext dbContext, IRepository<Book> bookRepository) : base(dbContext)
        {
            _bookRepository = bookRepository;
        }

        public BookTracking Reserve(BookTracking entity)
        {
            if (entity.BookingTypeId > 0 && !string.IsNullOrEmpty(entity.UserId) && entity.BookingTypeId == Convert.ToInt32(BookingType.Reserved))
            {
                var result = from bt in _dbContext.Set<BookTracking>()
                             where bt.BookingTypeId == Convert.ToInt32(BookingType.Reserved)
                             && bt.UserId == entity.UserId
                             select bt;
                //checking user reserved books count
                if (result.Count() < 3)
                {
                    result = result.Where(x => x.BookId == entity.BookId);

                    var book = _bookRepository.GetById(entity.BookId);
                    //reserve book quantity can not high from book stock qunatity
                    if (result.Sum(x => x.Qunatity) < book.Quantity)
                    {
                        entity.BookingTypeId = Convert.ToInt32(BookingType.Reserved);
                        entity.ReserveDate = DateTime.Now;
                        entity.ReturnDate = entity.ReserveDate.AddDays(5);
                        Add(entity);
                    }
                }
            }

            return entity;
        }

        public BookTracking Return(BookTracking entity)
        {
            //you can set return book only if exist reserved one 
            if (_dbContext.Set<BookTracking>().Any(x => x.UserId == entity.UserId
            && x.BookId == entity.BookId
            && x.BookingTypeId == Convert.ToInt32(BookingType.Reserved)
            ))
            {
                entity.ActualReturnDate = DateTime.Now;
                entity.BookingTypeId = Convert.ToInt32(BookingType.Returned);
                Add(entity);
            }

            return entity;
        }
    }
}
