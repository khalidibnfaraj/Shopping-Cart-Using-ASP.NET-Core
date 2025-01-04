﻿namespace myShop.Entities.Models;

public class IEnumerable
{
    public int Id { get; set; } 
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedTime { get; set; } = DateTime.Now;
}
