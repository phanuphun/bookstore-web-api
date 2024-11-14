using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SampleWebAPI.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        public string Image { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string Drawer { get; set; }
        public string Translater { get; set; }
        public double Price { get; set; }
        public string Pages { get; set; }
        public string Thickness { get; set; }
        public string Weight { get; set; }
        public int Amount { get; set; }
        public string Size { get; set; }
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; }
    }
}