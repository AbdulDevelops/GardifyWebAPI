using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GardifyNewsletter.Utilities
{
    public class Utilities
    {
        public static Guid APP_GUID = new Guid(Program.APPLICATION_ID);
    }
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }
        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            this.AddRange(items);

        }
        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageIndex < TotalPages);
            }
        }
        public int ThepageIndex
        {
            get
            {
                return PageIndex;
            }

        }
        public int TheTotalPages
        {
            
           
            get
            {
                if (TotalPages == 0)
                {
                    return TotalPages + 1;
                }
                return TotalPages;
            }

        }

        public static async Task<PaginatedList<T>>CreateAsync( IEnumerable<T> source, int pageIndex, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip(
                (pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
