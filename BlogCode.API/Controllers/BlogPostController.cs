using AutoMapper;
using BlogCode.API.Models.Domain;
using BlogCode.API.Models.DTO;
using BlogCode.API.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace BlogCode.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostController : ControllerBase
    {
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly ICategoryRespository _categoryRespository;
        ApiResponse _response;
        IMapper _mapper;
        public BlogPostController(IBlogPostRepository blogPostRepository, ICategoryRespository categoryRespository, IMapper mapper)
        {
            _blogPostRepository = blogPostRepository;
            _categoryRespository = categoryRespository;
            _response = new ApiResponse();
            _mapper = mapper;
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
                    UrlHandle = request.UrlHandle,
                    Categories = new List<Category>()
                };

                // Check and validate correct category id

                foreach (var categoryGUID in request.Categories)
                {
                    var getCategoryFromDb = await _categoryRespository.GetCategoryById(categoryGUID);
                    if (getCategoryFromDb is not null)
                    {
                        blogPost.Categories.Add(getCategoryFromDb);
                    }
                }

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
                    UrlHandle = getResponse.UrlHandle,
                    Categories = getResponse.Categories.Select(
                       x => new CategoryDto()
                       {
                           Id = x.Id,
                           Name = x.Name,
                           UrlHandle = x.UrlHandle
                       }).ToList()
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

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var getResponse = await _blogPostRepository.GetAllAsync();
                // Convert Domain model to Dto
                var response = new List<BlogPostDto>();

                foreach (var post in getResponse)
                {
                    response.Add(new BlogPostDto()
                    {
                        Id = post.Id,
                        Title = post.Title,
                        Content = post.Content,
                        Author = post.Author,
                        FeaturedImageUrl = post.FeaturedImageUrl,
                        IsVisible = post.IsVisible,
                        PublishedDate = post.PublishedDate,
                        ShortDescription = post.ShortDescription,
                        UrlHandle = post.UrlHandle,
                        Categories = post.Categories?.Select(x => new CategoryDto()
                        {
                            Id = x.Id,
                            Name = x.Name,
                            UrlHandle = x.UrlHandle
                        }).ToList()
                    });
                }

                if (response != null)
                {
                    _response.Result = response;
                    _response.IsSuccess = true;
                    _response.Message = "Success";
                    return Ok(_response);
                }
                else
                {
                    _response.Result = null;
                    _response.IsSuccess = false;
                    _response.Message = "NotFound";
                    return NotFound(_response);
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

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetBlogPostById([FromRoute] Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest();
            }
            // call the respository
            var post = await _blogPostRepository.GetBlogPostByIdAsync(id);
            if (post == null) { return NotFound(); }

            //Convert Doamin model to DTO

            BlogPostDto response = new BlogPostDto()
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                Author = post.Author,
                FeaturedImageUrl = post.FeaturedImageUrl,
                IsVisible = post.IsVisible,
                PublishedDate = post.PublishedDate,
                ShortDescription = post.ShortDescription,
                UrlHandle = post.UrlHandle,
                Categories = post.Categories?.Select(x => new CategoryDto()
                {
                    Id = x.Id,
                    Name = x.Name,
                    UrlHandle = x.UrlHandle
                }).ToList()
            };

            if (response != null)
            {
                _response.Result = response;
                _response.IsSuccess = true;
                _response.Message = "Success";
                return Ok(_response);
            }

            return Ok(_response);
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateBlogPostById([FromRoute] Guid id, [FromBody] UpdateBlogPostRequestDto request)
        {
            try
            {


                if (id == Guid.Empty)
                {
                    return BadRequest();
                }

                //Convert DTO to domain Model
                /* var blogPost = new BlogPost()
                 {
                     Id = request.Id,
                     Title = request.Title,
                     Content = request.Content,
                     Author = request.Author,
                     FeaturedImageUrl = request.FeaturedImageUrl,
                     IsVisible = request.IsVisible,
                     PublishedDate = request.PublishedDate,
                     ShortDescription = request.ShortDescription,
                     UrlHandle = request.UrlHandle,
                     Categories = new List<Category>()

                 };*/

                var blogPost = _mapper.Map<BlogPost>(request);
                blogPost.Id = id;
                // Get category and assign to list
                blogPost.Categories = new List<Category>();
                foreach (var categoryGuid in request.Categories)
                {
                    var getCategoryFromDb = await _categoryRespository.GetCategoryById(categoryGuid);
                    if (getCategoryFromDb != null)
                    {
                        blogPost.Categories.Add(getCategoryFromDb);
                    }
                }

                // call the respository for update
                var post = await _blogPostRepository.UpdateBlogPostAsync(blogPost);
                if (post == null)
                {
                    return NotFound();
                }

                // Convert Domain model to DTO
                var response = _mapper.Map<BlogPostDto>(post);
                if (response != null)
                {
                    _response.Result = response;
                    _response.IsSuccess = true;
                    _response.Message = "Success";
                    return Ok(_response);
                }
            }
            catch(Exception ex)
            {
                string error = ex.Message;
            }

            return Ok(_response);
        }
    }
}
