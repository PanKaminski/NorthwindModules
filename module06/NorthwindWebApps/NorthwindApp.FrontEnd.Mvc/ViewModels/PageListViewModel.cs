using System.Collections.Generic;

namespace NorthwindApp.FrontEnd.Mvc.ViewModels
{
    public class PageListViewModel<T>
    {
        public IEnumerable<T> Items { get; set; }

        public PageInfo PageInfo { get; set; }
    }
}
