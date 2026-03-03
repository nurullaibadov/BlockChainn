using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain.Application.Common
{
    public class PaginationQuery
    {
        private int _page = 1;
        private int _pageSize = 10;

        public int Page
        {
            get => _page;
            set => _page = value < 1 ? 1 : value;
        }

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > 100 ? 100 : value < 1 ? 10 : value;
        }

        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; } = true;
    }
}
