using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BestStoreMVC.Models;

public class CreateProductDto
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Brand { get; set; } = string.Empty;
    [Required, MaxLength(100)] 
    public string Category { get; set; } = string.Empty;
    
    [Required, MaxLength(100)] 
    public string Price { get; set; } = string.Empty;
    
    [Required, MaxLength(100)] 
    public string Description { get; set; } = string.Empty;

    public IFormFile ImageFile { get; set; } = default!;

}
