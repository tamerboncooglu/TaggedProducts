using System.Collections.Generic;

namespace TaggedProducts.Web.Models
{
    public class GridModel : BaseModel
    {
        public GridModel()
        {
            this.Paging = new PagingModel();
            this.TableHeaders = new List<string>();
        }

        public string Title { get; set; }
        public string Name { get; set; }
        public string Keyword { get; set; }

        public List<string> TableHeaders { get; set; }
        public string DataLinesAsHtmlTr { get; set; }

        public bool IsAddButtonVisible { get; set; }
        public string AddLink { get; set; }
        public bool IsDeleteButtonVisible { get; set; }

        public PagingModel Paging { get; set; }
    }
}