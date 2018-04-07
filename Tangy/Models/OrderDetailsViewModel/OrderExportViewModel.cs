using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Tangy.Models.OrderDetailsViewModel
{
    public class OrderExportViewModel
    {
        [Display(Name = "Start Date")]
        public DateTime startDate { get; set; }
        [Display(Name = "End Date")]
        public DateTime endDate { get; set; }
    }
}
