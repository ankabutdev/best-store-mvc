using BestStoreMVC.Models;
using BestStoreMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BestStoreMVC.Controllers;

public class ProductsController : Controller
{
    public AppDbContext _context;
    private IWebHostEnvironment _env;
    public ProductsController(AppDbContext context, IWebHostEnvironment env)
    {
        this._context = context;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _context.Products.OrderByDescending(x => x.Id).ToListAsync();
        return View(products);
    }

    public async Task<IActionResult> Create()
    {

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductDto dto)
    {
        if (dto.ImageFile == null)
        {
            ModelState.AddModelError("ImageFile", "The image file is required!");
        }

        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        // save file

        string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
        newFileName += Path.GetExtension(dto.ImageFile!.FileName);

        string imageFullPath = _env.WebRootPath + "/products/" + newFileName;
        using (var stream = System.IO.File.Create(imageFullPath))
        {
            await dto.ImageFile.CopyToAsync(stream);
        }

        var product = new Product()
        {
            Name = dto.Name,
            Brand = dto.Brand,
            Category = dto.Category,
            Price = dto.Price,
            Description = dto.Description,
            ImageFileName = newFileName,
            CreatedAt = DateTime.UtcNow,
        };

        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Products");
    }

    public async Task<IActionResult> Edit(int Id)
    {
        var product = await _context.Products.FindAsync(Id);
        if (product == null)
        {
            return RedirectToAction("Index", "Products");
        }

        var proudctDto = new UpdateProductDto()
        {
            Name = product.Name,
            Brand = product.Brand,
            Category = product.Category,
            Price = product.Price,
            Description = product.Description,
        };

        ViewData["ProductId"] = product.Id;
        ViewData["ImageFileName"] = product.ImageFileName;
        ViewData["CreatedAt"] = product.CreatedAt.ToString("MM/dd/yyyy");

        return View(proudctDto);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int Id, UpdateProductDto dto)
    {
        var product = await _context.Products.FindAsync(Id);
        if (product == null)
        {
            return RedirectToAction("Index", "Products");
        }

        if (!ModelState.IsValid)
        {
            ViewData["ProductId"] = product.Id;
            ViewData["ImageFileName"] = product.ImageFileName;
            ViewData["CreatedAt"] = product.CreatedAt.ToString("MM/dd/yyyy");
            return View(dto);
        }

        string newImageFile = product.ImageFileName;
        if (dto.ImageFile != null)
        {
            newImageFile = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
            newImageFile += Path.Combine(dto.ImageFile.FileName);

            string imageFullPath = _env.WebRootPath + "/products/" + newImageFile;
            using (var stream = System.IO.File.Create(imageFullPath))
            {
                await dto.ImageFile.CopyToAsync(stream);
            }

            string oldImageFullPath = _env.WebRootPath + "/products/" + product.ImageFileName;
            System.IO.File.Delete(oldImageFullPath);
        }

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Brand = dto.Brand;
        product.Category = dto.Category;
        product.Price = dto.Price;
        product.ImageFileName = newImageFile;

        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Products");
    }

    public async Task<IActionResult> Delete(int Id)
    {
        var product = await _context.Products.FindAsync(Id);
        if (product == null)
        {
            return RedirectToAction("Index", "Products");
        }

        string imageFullPath = _env.WebRootPath + "/products/" + product.ImageFileName;
        System.IO.File.Delete(imageFullPath);

        _context.Products.Remove(product);
        await _context.SaveChangesAsync(true);

        return RedirectToAction("Index", "Products");
    }
}
