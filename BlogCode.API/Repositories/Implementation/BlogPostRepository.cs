using BlogCode.API.Data;
using BlogCode.API.Models.Domain;
using BlogCode.API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace BlogCode.API.Repositories.Implementation
{
    public class BlogPostRepository : IBlogPostRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public BlogPostRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<BlogPost> CreateAsync(BlogPost blogPost)
        {
            await _dbContext.BlogPosts.AddAsync(blogPost);
            await _dbContext.SaveChangesAsync();
            return blogPost;
        }

        public async Task<IEnumerable<BlogPost>> GetAllAsync()
        {
            return await _dbContext.BlogPosts.Include(x => x.Categories).ToListAsync();
        }

        public async Task<BlogPost> GetBlogPostByIdAsync(Guid id)
        {
            return await _dbContext.BlogPosts.Include(x => x.Categories).FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<BlogPost> GetBlogPostByUrlHandleAsync(string urlHandle)
        {
            return await _dbContext.BlogPosts.Include(x => x.Categories).FirstOrDefaultAsync(u => u.UrlHandle == urlHandle);
        }
        public async Task<BlogPost?> UpdateBlogPostAsync(BlogPost blogPost)
        {
            var getPostFromDb = await _dbContext.BlogPosts.Include(u => u.Categories).FirstOrDefaultAsync(u => u.Id == blogPost.Id);
            if (getPostFromDb != null)
            {
                // Update the blog post
                _dbContext.Entry(getPostFromDb).CurrentValues.SetValues(blogPost);

                // Update Categories
                getPostFromDb.Categories = blogPost.Categories;
                await _dbContext.SaveChangesAsync();
                return blogPost;
            }
            else
            {
                return null;
            }
        }
    }
}
