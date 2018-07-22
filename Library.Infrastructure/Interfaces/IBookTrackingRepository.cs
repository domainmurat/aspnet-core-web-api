using Library.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Infrastructure.Interfaces
{
    public interface IBookTrackingRepository : IRepository<BookTracking>, IAsyncRepository<BookTracking>
    {
        BookTracking Reserve(BookTracking entity);
        BookTracking Return(BookTracking entity);
    }
}
