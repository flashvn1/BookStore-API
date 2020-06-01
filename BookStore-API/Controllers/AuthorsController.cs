using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AutoMapper;
using BookStore_API.Contracts;
using BookStore_API.Data;
using BookStore_API.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BookStore_API.Controllers
{
    /// <summary>
    /// Endpoint used to interact with Authors in the book store's database
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _authRepository;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;

        public AuthorsController(IAuthorRepository authRepository, ILoggerService logger, IMapper mapper)
        {
            _authRepository = authRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Get All Authors
        /// </summary>
        /// <returns>List of Authors</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthors()
        {
            try
            {

                _logger.LogInfo("Attempted Get All Authors");
                var authors = await _authRepository.FindAll();
                var response = _mapper.Map<IList<AuthorDTO>>(authors);
                _logger.LogInfo("Successfully got all Authors");
                return Ok(response);
            }
            catch (Exception e)
            {
                return internalError($"{e.Message} - {e.InnerException}");
            }

        }

        /// <summary>
        /// Get a specific Author given an id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>An Author record</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthor(int id)
        {
            try
            {
                _logger.LogInfo($"Attempted Get Author with id:{id}");
                var author = await _authRepository.FindById(id);
                if (author == null)
                {
                    _logger.LogWarn($"Author with id:{id} not found");
                    return NotFound($"Author with id:{id} not found");
                }
                var response = _mapper.Map<AuthorDTO>(author);
                _logger.LogInfo($"Successfully got Author with id:{id}");
                return Ok(response);
            }
            catch (Exception e)
            {
                return internalError($"{e.Message} - {e.InnerException}");
            }

        }

        /// <summary>
        /// Create an Author
        /// </summary>
        /// <param name="authorDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] AuthorCreateDTO authorDTO)
        {
            try
            {
                _logger.LogInfo($"Author submission was atempted");
                if (authorDTO == null)
                {
                    _logger.LogWarn($"Empty Request Was Submitted");
                    return BadRequest(ModelState);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarn($"Author Data Was Incomplete");
                    return BadRequest(ModelState);
                }
                var author = _mapper.Map<Author>(authorDTO);
                var isSuccess = await _authRepository.Create(author);
                if (!isSuccess)
                {
                    return internalError($"Author creation failed");
                }
                _logger.LogInfo($"Author created");
                return Created("Create", new { author });
            }
            catch (Exception e)
            {
                return internalError($"{e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Update Author
        /// </summary>
        /// <param name="id"></param>
        /// <param name="authorDTO"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] AuthorUpdateDTO authorDTO)
        {
            try
            {
                _logger.LogInfo($"Author updated with id: {id} was atempted");
                if (id < 0 || authorDTO == null || id != authorDTO.Id)
                {
                    _logger.LogWarn($"Author Data Was Incomplete");
                    return BadRequest();
                }
                var isExists = await _authRepository.isExists(id);
                if(!isExists)
                {
                    _logger.LogWarn($"Author with id: {id} Data Was Not Found");
                    return NotFound();
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarn($"Author Data Was Incomplete");
                    return BadRequest(ModelState);
                }
                var author = _mapper.Map<Author>(authorDTO);
                var isSuccess = await _authRepository.Update(author);
                if (!isSuccess)
                {
                    return internalError($"Author Update with id: {id} failed");
                }
                _logger.LogInfo($"Author with id: {id}  Updated");
                return NoContent();
            }
            catch (Exception e)
            {

                return internalError($"{e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Delete an Author Record
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                _logger.LogInfo($"Author delete with id: {id} was atempted");
                if (id < 0 )
                {
                    _logger.LogWarn($"Author Data Was Incomplete");
                    return BadRequest();
                }
                var isExists = await _authRepository.isExists(id);
                if (!isExists)
                {
                    _logger.LogWarn($"Author with id: {id} Data Was Not Found");
                    return NotFound();
                }
                var author = await _authRepository.FindById(id);
                var isSuccess = await _authRepository.Delete(author);
                if(!isSuccess)
                {
                    return internalError($"Author Delete with id: {id} failed");
                }
                _logger.LogInfo($"Author with id: {id}  Deleted");
                return NoContent();

            }
            catch (Exception e)
            {

                return internalError($"{e.Message} - {e.InnerException}");
            }
        }


        private ObjectResult internalError(string message)
        {
            _logger.LogError(message);
            return StatusCode(500, "Something went wrong. Please contact the administrator");
        }

    }
}
