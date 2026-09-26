using Microsoft.AspNetCore.Mvc;
using ServiceRepoDemo.Models;
using ServiceRepoDemo.Services;

namespace ServiceRepoDemo.Controllers;

public class ProductsController : Controller
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _productService.GetAllAsync();
        return View(products);
    }

    public IActionResult Create() => View(new Product());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product)
    {
        if (!ModelState.IsValid)
            return View(product);

        product.Name = product.Name.Trim();
        product.CreatedAt = DateTime.UtcNow;

        var result = await _productService.CreateAsync(product);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            return View(product);
        }

        TempData["Message"] = "Product created.";
        return RedirectToAction(nameof(Index));
    }

    //public async Task<IActionResult> Edit(int id)
    //{
    //    var product = await _productService.GetByIdAsync(id);

    //    if (product == null)
    //        return NotFound();
    //    return View(product);
    //}

    public async Task<IActionResult> Edit(int id)
    {
        var product = await _productService.GetByIdAsync(id);

        return product is null
            ? NotFound()
            : View(product);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Product product)
    {
        if (id != product.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(product);

        var result = await _productService.UpdateAsync(product);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            return View(product);
        }

        TempData["Message"] = "Product updated.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var product = await _productService.GetByIdAsync(id);

        return product is null
            ? NotFound()
            : View(product);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var result = await _productService.DeleteAsync(id);

        if (!result.Success)
        {
            TempData["Message"] = result.Error;
            return RedirectToAction(nameof(Index));
        }

        TempData["Message"] = "Product deleted.";
        return RedirectToAction(nameof(Index));
    }
}