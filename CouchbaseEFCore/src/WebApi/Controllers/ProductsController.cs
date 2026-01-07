using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductService productService, ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    /// <summary>
    /// Get all products
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
    {
        var products = await _productService.GetAllAsync();
        return Ok(products);
    }

    /// <summary>
    /// Get product by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> GetById(string id)
    {
        var product = await _productService.GetByIdAsync(id);
        
        if (product == null)
        {
            return NotFound(new { message = $"Product with ID '{id}' not found" });
        }

        return Ok(product);
    }

    /// <summary>
    /// Get products by category
    /// </summary>
    [HttpGet("category/{category}")]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetByCategory(string category)
    {
        var products = await _productService.GetByCategoryAsync(category);
        return Ok(products);
    }

    /// <summary>
    /// Get active products
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetActiveProducts()
    {
        var products = await _productService.GetActiveProductsAsync();
        return Ok(products);
    }

    /// <summary>
    /// Search products by name
    /// </summary>
    [HttpGet("search/{searchTerm}")]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductDto>>> SearchByName(string searchTerm)
    {
        var products = await _productService.SearchByNameAsync(searchTerm);
        return Ok(products);
    }

    // ====================================================================
    // NEW ENDPOINTS FOR NESTED OBJECTS
    // ====================================================================

    /// <summary>
    /// Get products by warehouse city
    /// </summary>
    [HttpGet("warehouse/city/{city}")]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetByWarehouseCity(string city)
    {
        var products = await _productService.GetByWarehouseCityAsync(city);
        return Ok(products);
    }

    /// <summary>
    /// Get products by warehouse code
    /// </summary>
    [HttpGet("warehouse/code/{code}")]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetByWarehouseCode(string code)
    {
        var products = await _productService.GetByWarehouseCodeAsync(code);
        return Ok(products);
    }

    /// <summary>
    /// Get products by specification key and value
    /// </summary>
    [HttpGet("specification/{key}/{value}")]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetBySpecification(string key, string value)
    {
        var products = await _productService.GetBySpecificationAsync(key, value);
        return Ok(products);
    }

    /// <summary>
    /// Get products near coordinates (within radius)
    /// </summary>
    [HttpGet("nearby")]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetNearbyProducts(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] double radiusKm = 50)
    {
        var products = await _productService.GetNearbyProductsAsync(latitude, longitude, radiusKm);
        return Ok(products);
    }

    // ====================================================================
    // CREATE, UPDATE, DELETE
    // ====================================================================

    /// <summary>
    /// Create a new product (with nested objects)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto createDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var product = await _productService.CreateAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    /// <summary>
    /// Update an existing product (with nested objects)
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDto>> Update(string id, [FromBody] UpdateProductDto updateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var product = await _productService.UpdateAsync(id, updateDto);
        
        if (product == null)
        {
            return NotFound(new { message = $"Product with ID '{id}' not found" });
        }

        return Ok(product);
    }

    /// <summary>
    /// Update product warehouse
    /// </summary>
    [HttpPut("{id}/warehouse")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> UpdateWarehouse(
        string id, 
        [FromBody] WarehouseLocationDto warehouseDto)
    {
        var product = await _productService.UpdateWarehouseAsync(id, warehouseDto);
        
        if (product == null)
        {
            return NotFound(new { message = $"Product with ID '{id}' not found" });
        }

        return Ok(product);
    }

    /// <summary>
    /// Add specification to product
    /// </summary>
    [HttpPost("{id}/specifications")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> AddSpecification(
        string id,
        [FromBody] ProductSpecificationDto specificationDto)
    {
        var product = await _productService.AddSpecificationAsync(id, specificationDto);
        
        if (product == null)
        {
            return NotFound(new { message = $"Product with ID '{id}' not found" });
        }

        return Ok(product);
    }

    /// <summary>
    /// Remove specification from product
    /// </summary>
    [HttpDelete("{id}/specifications/{key}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> RemoveSpecification(string id, string key)
    {
        var product = await _productService.RemoveSpecificationAsync(id, key);
        
        if (product == null)
        {
            return NotFound(new { message = $"Product with ID '{id}' not found" });
        }

        return Ok(product);
    }

    /// <summary>
    /// Delete a product
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _productService.DeleteAsync(id);
        
        if (!result)
        {
            return NotFound(new { message = $"Product with ID '{id}' not found" });
        }

        return NoContent();
    }
}
