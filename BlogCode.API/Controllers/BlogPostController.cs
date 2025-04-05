using BlogCode.API.Models.Domain;
using BlogCode.API.Models.DTO;
using BlogCode.API.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogCode.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostController : ControllerBase
    {
        private readonly IBlogPostRepository _blogPostRepository;
        ApiResponse _response;
        public BlogPostController(IBlogPostRepository blogPostRepository)
        {
            _blogPostRepository = blogPostRepository;
            _response = new ApiResponse();
        }
        [HttpPost]
        public async Task<IActionResult> CreateBlogPost([FromBody] CreateBlogPostRequestDto request)
        {
            try
            {
                if (request == null) { return BadRequest(); }

                //Convert Dto to Domain model
                var blogPost = new BlogPost()
                {
                    Author = request.Author,
                    Content = request.Content,
                    FeaturedImageUrl = request.FeaturedImageUrl,
                    Title = request.Title,
                    IsVisible = request.IsVisible,
                    PublishedDate = request.PublishedDate,
                    ShortDescription = request.ShortDescription,
                    UrlHandle = request.UrlHandle
                };

                // Call the repository method
                var getResponse = await _blogPostRepository.CreateAsync(blogPost);
                // Convert Domain model to Dto

                var response = new BlogPostDto()
                {
                    Id = getResponse.Id,
                    Author = getResponse.Author,
                    Content = getResponse.Content,
                    FeaturedImageUrl = getResponse.FeaturedImageUrl,
                    Title = getResponse.Title,
                    IsVisible = getResponse.IsVisible,
                    PublishedDate = getResponse.PublishedDate,
                    ShortDescription = getResponse.ShortDescription,
                    UrlHandle = getResponse.UrlHandle
                };

                if (response != null)
                {
                    _response.Result = response;
                    _response.IsSuccess = true;
                    _response.Message = "Record inserted successfully.";
                }
                else
                {
                    _response.Result = null;
                    _response.IsSuccess = false;
                    _response.Message = "Record does not insert successfully.";
                }

            }
            catch (Exception ex)
            {
                _response.Result = null;
                _response.IsSuccess = false;
                _response.Message = "Error occured";
                _response.ErrorMessage.Add(ex.Message);
            }
            
            return Ok(_response);
        }
    }
}
