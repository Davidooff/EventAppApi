using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class CategoryService
{
    private readonly IDatabaseContext _context;
    public CategoryService(IDatabaseContext databaseContext)
    {
        _context = databaseContext;
    }

    public Task<Category[]> GetAllCategories()
    {
        return _context.Categories.ToArrayAsync();
    }
    
    public Task AddCategory(Category category)
    {
        _context.Categories.Add(category);
        return _context.SaveChangesAsync();
    }
    
    public Task UpdateCategory(Category category)
    {
        _context.Categories.Update(category);
        return _context.SaveChangesAsync();
    }
    
    public Task RemoveCategory(Category category)
    {
        _context.Categories.Remove(category);
        return _context.SaveChangesAsync();
    }
}