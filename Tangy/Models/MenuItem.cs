using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Tangy.Models
{
    public class MenuItem
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
        public string Image { get; set; }
        public string Spicyness { get; set; }
        public enum EScipy { NA=0,Spicy=1,VerySpicy=2}

        [Range(1,int.MaxValue,ErrorMessage ="Price should be greater than ${1}")]
        public double Price { get; set; }

        [Display(Name ="Category")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }


        [Display(Name = "SubCategory")]
        public int SubCategoryId { get; set; }

        [ForeignKey("SubCategoryId")]
        public virtual SubCategory SubCategory { get; set; }

    }
}
