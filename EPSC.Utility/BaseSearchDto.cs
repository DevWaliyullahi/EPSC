using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPSC.Utility
{
    public class BaseSearchDto
    {
        public int PageSize { get; set; } = 10;
        public int PageNumber { get; set; } = 1;
        public string SearchValue { get; set; } = string.Empty;  
    }
}
