using Gumo.Core.Entities;
using Gumo.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gumo.Web.Controllers;

public class MenuController(GumoDbContext db) : Controller
{
    public async Task<IActionResult> Index(int? categoryId)
    {
        var categories = await db.Categories
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();

        var query = db.MenuItems
            .Include(m => m.Category)
            .Where(m => m.IsAvailable)
            .OrderBy(m => m.Category.DisplayOrder)
            .ThenBy(m => m.DisplayOrder)
            .AsQueryable();

        if (categoryId.HasValue)
            query = query.Where(m => m.CategoryId == categoryId.Value);

        var items = await query.ToListAsync();

        ViewBag.Categories = categories;
        ViewBag.SelectedCategoryId = categoryId;
        return View(items);
    }
}