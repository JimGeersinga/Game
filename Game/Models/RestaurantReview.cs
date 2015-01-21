using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Game.Models
{
    public class RestaurantReview
    {
        public int Id { get; set; }

        [Range(1,10)]
        public int Rating { get; set; }

        [Required]
        [StringLength(1024)]
        public string Body { get; set; }

        [Display(Name="User Name")]
        [DisplayFormat(NullDisplayText="Anonymous")]
        public string ReviewerName { get; set; }
        public int RestaurantId { get; set; }       
    }
}