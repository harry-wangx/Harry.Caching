using System;

namespace Sample.Models
{
    public class ProductModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime CreationTime { get; set; } = DateTime.Now;
    }
}
