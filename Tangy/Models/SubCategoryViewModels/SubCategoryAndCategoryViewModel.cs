using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Tangy.Models.SubCategoryViewModels
{
    public class SubCategoryAndCategoryViewModel
    {
        public SubCategory SubCategory { get; set; }
        public IEnumerable<Category> CategoryList { get; set; }

        public List<string> SubCategoryList { get; set; }

        [Display(Name ="New Sub Category")]
        public bool isNew { get; set; }

        public string StatusMessage { get; set; }

    }
}
