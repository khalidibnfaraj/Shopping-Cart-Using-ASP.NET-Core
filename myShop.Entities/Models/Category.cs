using System.ComponentModel;

namespace myShop.Entities.Models;

public class Category
{
    public int Id { get; set; }
    [DisplayName("Name")]
    public string Name { get; set; }
    [DisplayName("Description")]
    public string Description { get; set; }
    public DateTime CreatedTime { get; set; } = DateTime.Now;
}
