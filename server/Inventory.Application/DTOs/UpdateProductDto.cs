namespace Inventory.Application.DTOs;

public class UpdateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}
