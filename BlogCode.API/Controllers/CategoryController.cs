using BlogCode.API.Models.Domain;
using BlogCode.API.Models.DTO;
using BlogCode.API.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogCode.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRespository _categoryRespository;
        public CategoryController(ICategoryRespository categoryRespository)
        {
            _categoryRespository = categoryRespository;
        }
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequestDto request)
        {
            if (string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.UrlHandle))
            {
                return BadRequest();
            }

            // Convert DTO  to Domain model

            var category = new Category
            {
                Name = request.Name,
                UrlHandle = request.UrlHandle
            };

            var categoryFromDb = await _categoryRespository.CreateCategory(category);

            // Convert Domain model to DTO
            var response = new CategoryDto
            {
                Id = categoryFromDb.Id,
                Name = categoryFromDb.Name,
                UrlHandle = categoryFromDb.UrlHandle
            };

            return Ok(response);

        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryRespository.GetAllCategories();
            var response = new List<CategoryDto>();
            foreach (var category in categories)
            {
                response.Add(new CategoryDto() { Id = category.Id, Name = category.Name, UrlHandle = category.UrlHandle });
            }
            return Ok(response);
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetCategory([FromRoute] Guid id)
        {
            if (id == Guid.Empty) return BadRequest();

            var getCategory = await _categoryRespository.GetCategoryById(id);

            if (getCategory == null) return NotFound();

            var response = new CategoryDto
            {
                Id = getCategory.Id,
                Name = getCategory.Name,
                UrlHandle = getCategory.UrlHandle
            };

            return Ok(response);
        }

        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> EditCategory([FromRoute] Guid id, [FromBody] UpdateCategoryRequestDto request)
        {
            if (string.IsNullOrEmpty(id.ToString()) || string.IsNullOrEmpty(request.Name))
            {
                return BadRequest();
            }
            // Convert Dto to Domain model

            var category = new Category
            {
                Id = id,
                Name = request.Name,
                UrlHandle = request.UrlHandle
            };

            var getCategory = await _categoryRespository.UpdateCategoryAsync(category);
            if (getCategory == null) return NotFound();

            // Convert Domain model to Dto
            var response = new CategoryDto
            {
                Id = getCategory.Id,
                Name = getCategory.Name,
                UrlHandle = getCategory.UrlHandle
            };
            return Ok(response);
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeleteCategory([FromRoute] Guid id)
        {
            if(id == null)
            {
                return BadRequest();
            }
            var getCategory = await _categoryRespository.DeleteAsync(id);
            if (getCategory == null) return NotFound();
            // Convert Domain model to DTO
            var response = new CategoryDto()
            {
                Id = getCategory.Id,
                Name= getCategory.Name,
                UrlHandle = getCategory.UrlHandle
            };
            return Ok(response);
        }
    }
}
