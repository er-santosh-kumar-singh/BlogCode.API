using BlogCode.API.Models.Domain;

namespace BlogCode.API.Repositories.Interface
{
    public interface ICategoryRespository
    {
        Task<Category> CreateCategory(Category category);
        Task<IEnumerable<Category>> GetAllCategories();
    }
}
