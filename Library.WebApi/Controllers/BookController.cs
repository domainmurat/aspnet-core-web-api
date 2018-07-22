using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.Infrastructure.Entities;
using Library.Infrastructure.Interfaces;
using Library.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.WebApi.Controllers
{
    //[Authorize]
    [Route("[controller]/[action]")]
    public class BookController : Controller
    {
        private readonly IAsyncRepository<Book> _bookRepository;
        private readonly IBookTrackingRepository _bookTrackingRepository;

        public BookController(IAsyncRepository<Book> bookRepository, IBookTrackingRepository bookTrackingRepository)
        {
            _bookRepository = bookRepository;
            _bookTrackingRepository = bookTrackingRepository;
        }

        [HttpGet]
        public async Task<List<Book>> GetAll()
        {
            return await _bookRepository.ListAllAsync();
        }

        [HttpGet]
        public async Task<Book> SaveBook([FromBody] BookModel bookModel)
        {
            Book book = new Book();
            book.Id = bookModel.Id;
            book.Name = bookModel.Name;
            book.Quantity = bookModel.Quantity;

            if (book.Id > 0)
            {
                await _bookRepository.AddAsync(book);
            }
            else
            {
                await _bookRepository.UpdateAsync(book);
            }
            return book;

        }

        [HttpPost]
        public ActionResult<BookTrackingModel> Reserve([FromBody]BookTrackingSaveModel bookTrackingSaveModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(bookTrackingSaveModel);
            }
            BookTracking bookTracking = new BookTracking();
            bookTracking.BookId = bookTrackingSaveModel.BookId;
            bookTracking.UserId = bookTrackingSaveModel.UserId;
            bookTracking.Qunatity = bookTrackingSaveModel.Qunatity;
            bookTracking.BookingTypeId = bookTrackingSaveModel.BookingTypeId;

            return Ok(_bookTrackingRepository.Reserve(bookTracking));
        }

        [HttpPost]
        public ActionResult<BookTrackingModel> Return([FromBody]BookTrackingSaveModel bookTrackingSaveModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(bookTrackingSaveModel);
            }
            BookTracking bookTracking = new BookTracking();
            bookTracking.BookId = bookTrackingSaveModel.BookId;
            bookTracking.UserId = bookTrackingSaveModel.UserId;
            bookTracking.Qunatity = bookTrackingSaveModel.Qunatity;
            bookTracking.BookingTypeId = bookTrackingSaveModel.BookingTypeId;

            return Ok(_bookTrackingRepository.Return(bookTracking));
        }

        [HttpPost("{userId}")]
        public async Task<List<BookTrackingModel>> UserBook(string userId)
        {
            var bookTrackings = await _bookTrackingRepository.ListAsync(x => x.UserId == userId);
            return bookTrackings.Select(x => new BookTrackingModel()
            {
                BookId = x.BookId,
                BookTrackingId = x.Id,
                UserId = x.UserId,
                BookName = x.Book.Name,
                UserName = x.ApplicationUser.UserName,
                TotalQunatity = x.Book.Quantity,
                ReserveDate = x.ReserveDate,
                ReturnDate = x.ReturnDate
            }).ToList();
        }
    }
}