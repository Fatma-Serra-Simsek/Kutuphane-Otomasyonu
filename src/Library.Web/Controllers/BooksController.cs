using Library.Business.Dtos;
using Library.Business.Services;
using Library.Domain.Common;
using Library.Domain.Enums;
using Library.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Library.Web.Controllers;

public class BooksController : Controller
{
    private readonly IBookService _books;
    private readonly ICatalogLookupService _lookups;

    public BooksController(IBookService books, ICatalogLookupService lookups)
    {
        _books = books;
        _lookups = lookups;
    }

    public async Task<IActionResult> Index(string? term, int? categoryId, int page = 1, CancellationToken cancellationToken = default)
    {
        var result = await _books.SearchAsync(new SearchQuery
        {
            Term = term,
            CategoryId = categoryId,
            Page = page,
            PageSize = SearchQuery.DefaultPageSize
        }, cancellationToken);

        ViewBag.Categories = await BuildCategorySelectList(categoryId, cancellationToken);
        return View(new BookListViewModel
        {
            Term = term,
            CategoryId = categoryId,
            Result = result
        });
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        await PopulateLookups(cancellationToken);
        return View(new BookFormViewModel { PublishYear = DateTime.Today.Year });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await PopulateLookups(cancellationToken, model.AuthorIds);
            return View(model);
        }

        var result = await _books.CreateAsync(new BookCreateDto
        {
            Title = model.Title,
            ISBN = model.ISBN,
            PublishYear = model.PublishYear,
            Description = model.Description,
            CoverImageUrl = model.CoverImageUrl,
            CategoryId = model.CategoryId,
            PublisherId = model.PublisherId,
            AuthorIds = model.AuthorIds
        }, cancellationToken);

        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Kayıt başarısız.");
            await PopulateLookups(cancellationToken, model.AuthorIds);
            return View(model);
        }

        TempData["Success"] = "Kitap eklendi.";
        return RedirectToAction(nameof(Details), new { id = result.Value!.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var book = await _books.GetByIdAsync(id, cancellationToken);
        if (book is null)
            return NotFound();

        var model = new BookFormViewModel
        {
            Id = book.Id,
            Title = book.Title,
            ISBN = book.ISBN,
            PublishYear = book.PublishYear,
            Description = book.Description,
            CoverImageUrl = book.CoverImageUrl,
            CategoryId = book.CategoryId,
            PublisherId = book.PublisherId,
            AuthorIds = book.BookAuthors.Select(ba => ba.AuthorId).ToList()
        };
        await PopulateLookups(cancellationToken, model.AuthorIds);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BookFormViewModel model, CancellationToken cancellationToken)
    {
        if (id != model.Id)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            await PopulateLookups(cancellationToken, model.AuthorIds);
            return View(model);
        }

        var result = await _books.UpdateAsync(new BookUpdateDto
        {
            Id = model.Id,
            Title = model.Title,
            ISBN = model.ISBN,
            PublishYear = model.PublishYear,
            Description = model.Description,
            CoverImageUrl = model.CoverImageUrl,
            CategoryId = model.CategoryId,
            PublisherId = model.PublisherId,
            AuthorIds = model.AuthorIds
        }, cancellationToken);

        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Güncelleme başarısız.");
            await PopulateLookups(cancellationToken, model.AuthorIds);
            return View(model);
        }

        TempData["Success"] = "Kitap güncellendi.";
        return RedirectToAction(nameof(Details), new { id });
    }

    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var book = await _books.GetByIdAsync(id, cancellationToken);
        if (book is null)
            return NotFound();

        return View(book);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _books.SoftDeleteAsync(id, cancellationToken);
        if (!result.Succeeded)
            TempData["Error"] = result.Error;
        else
            TempData["Success"] = "Kitap pasife alındı.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddCopy(int bookId, string barcode, CancellationToken cancellationToken)
    {
        var result = await _books.AddCopyAsync(new BookCopyCreateDto
        {
            BookId = bookId,
            Barcode = barcode
        }, cancellationToken);

        if (!result.Succeeded)
            TempData["Error"] = result.Error;
        else
            TempData["Success"] = "Kopya eklendi.";

        return RedirectToAction(nameof(Details), new { id = bookId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeCopyStatus(int bookId, int copyId, CopyStatus status, CancellationToken cancellationToken)
    {
        var result = await _books.ChangeCopyStatusAsync(copyId, status, cancellationToken);
        if (!result.Succeeded)
            TempData["Error"] = result.Error;
        else
            TempData["Success"] = "Kopya durumu güncellendi.";

        return RedirectToAction(nameof(Details), new { id = bookId });
    }

    private async Task PopulateLookups(CancellationToken cancellationToken, IList<int>? selectedAuthors = null)
    {
        ViewBag.Categories = (await _lookups.ListCategoriesAsync(cancellationToken))
            .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
            .ToList();
        ViewBag.Publishers = (await _lookups.ListPublishersAsync(cancellationToken))
            .Select(p => new SelectListItem(p.Name, p.Id.ToString()))
            .ToList();
        ViewBag.Authors = (await _lookups.ListAuthorsAsync(cancellationToken))
            .Select(a => new SelectListItem(a.FullName, a.Id.ToString(), selectedAuthors?.Contains(a.Id) == true))
            .ToList();
    }

    private async Task<List<SelectListItem>> BuildCategorySelectList(int? selectedId, CancellationToken cancellationToken)
    {
        var items = (await _lookups.ListCategoriesAsync(cancellationToken))
            .Select(c => new SelectListItem(c.Name, c.Id.ToString(), selectedId == c.Id))
            .ToList();
        items.Insert(0, new SelectListItem("Tüm kategoriler", ""));
        return items;
    }
}
