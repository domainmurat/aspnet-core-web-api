using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.Infrastructure.Entities;
using Library.Infrastructure.Interfaces;
using Library.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Library.WebApi.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class BookController : Controller
    {
        private readonly IAsyncRepository<Book> _bookRepository;
        private readonly IBookTrackingRepository _bookTrackingRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookController(IAsyncRepository<Book> bookRepository, IBookTrackingRepository bookTrackingRepository,
                UserManager<ApplicationUser> userManager)
        {
            _bookRepository = bookRepository;
            _bookTrackingRepository = bookTrackingRepository;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<List<Book>> GetAll()
        {
            return await _bookRepository.ListAllAsync();
        }

        [HttpPost]
        public async Task<Book> SaveBook([FromBody] BookModel bookModel)
        {
            Book book = new Book();
            book.Id = bookModel.Id;
            book.Name = bookModel.Name;
            book.Quantity = bookModel.Quantity;

            if (book.Id > 0)
            {
                await _bookRepository.UpdateAsync(book);
            }
            else
            {
                await _bookRepository.AddAsync(book);
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
            try
            {
                List<BookTrackingModel> bookTrackingModels = new List<BookTrackingModel>();
                var bookTrackings = await _bookTrackingRepository.ListAsync(x => x.UserId == userId);

                foreach (var item in bookTrackings)
                {
                    BookTrackingModel bookTrackingModel = new BookTrackingModel();
                    var book = await _bookRepository.GetByIdAsync(item.BookId);
                    var user = await _userManager.FindByIdAsync(item.UserId);

                    bookTrackingModel.BookId = item.BookId;
                    bookTrackingModel.BookTrackingId = item.Id;
                    bookTrackingModel.UserId = item.UserId;
                    bookTrackingModel.BookName = book != null ? book.Name : string.Empty;
                    bookTrackingModel.UserName = user != null ? user.UserName : string.Empty;
                    bookTrackingModel.TotalQunatity = book != null ? book.Quantity : 0;
                    bookTrackingModel.ReserveDate = item.ReserveDate;
                    bookTrackingModel.ReturnDate = item.ReturnDate;

                    bookTrackingModels.Add(bookTrackingModel);
                }
                return bookTrackingModels;
            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}