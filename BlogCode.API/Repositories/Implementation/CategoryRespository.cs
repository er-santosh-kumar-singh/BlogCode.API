using BlogCode.API.Data;
using BlogCode.API.Models.Domain;
using BlogCode.API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace BlogCode.API.Repositories.Implementation
{
    public class CategoryRespository : ICategoryRespository
    {
        private readonly ApplicationDbContext _dbContext;
        public CategoryRespository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Category> CreateCategory(Category category)
        {
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();
            return category;
        }

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            return await _dbContext.Categories.ToListAsync();
        }

        public Task<Category> GetCategoryById(Guid id)
        {
            return _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Category> UpdateCategoryAsync(Category category)
        {
            var getCategoryFromDb = await _dbContext.Categories.FirstOrDefaultAsync(u => u.Id == category.Id);
            if (getCategoryFromDb != null)
            {
                _dbContext.Entry(getCategoryFromDb).CurrentValues.SetValues(category);
                await _dbContext.SaveChangesAsync();
                return category;
            }
            return null;

        }
    }
}
