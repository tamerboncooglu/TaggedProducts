namespace Mutfak.Web.Models
{
    public class PagingModel
    {
        public int PageIndex { get; set; }
        public int TotalPageCount { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
        public string PageUrl { get; set; }
        public bool IsAjaxPaging { get; set; }
    }
}