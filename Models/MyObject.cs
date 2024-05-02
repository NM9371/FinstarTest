using System.ComponentModel.DataAnnotations;

namespace FinstarTest.Models
{
    public class MyObject
    {
        [Key]
        public int Id { get; set; }
        public int Code { get; set; }
        public string Value { get; set; }
    }
}