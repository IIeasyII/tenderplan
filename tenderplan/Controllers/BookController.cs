using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using tenderplan.Data;
using tenderplan.Models;

namespace tenderplan.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "User")]
    public class BookController : ControllerBase
    {
        public readonly ApplicationContext _db;

        private Guid UserId => Guid.Parse(User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value);

        public BookController(ApplicationContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Добавить книгу в библиотеку
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Book>> Add(Book book)
        {
            try
            {
                if (book == null)
                {
                    return BadRequest();
                }

                var bookRep = await _db.Books.FindAsync(book.Id);

                if(bookRep != null)
                {
                    ModelState.AddModelError("id", "Книга уже добавлена в библиотеку");
                    return BadRequest(ModelState);
                }

                //Желательно иметь BookRepository
                var createBook = _db.Books.Add(book).Entity;

                await _db.SaveChangesAsync();
                //

                return CreatedAtAction(nameof(Add),
                    new { id = createBook.Id}, createBook);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "При получении записи из базы данных");
            }
        }

        /// <summary>
        /// Информация о книге
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> Details(long id)
        {
            var book = await _db.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        /// <summary>
        /// Обновить информацию о книге
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, Book book)
        {
            if(id != book.Id)
            {
                return BadRequest();
            }

            _db.Entry(book).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                var bookRep = await _db.Books.FindAsync(id);

                if (bookRep == null)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Удаление книги из библиотеки
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var book = await _db.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _db.Books.Remove(book);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Получить список книг по жанру
        /// </summary>
        [HttpGet("Genre/{genre}")]
        public async Task<ActionResult<List<Book>>> BooksByGenre(string genre)
        {
            var books = _db.Books.AsEnumerable().Where(b => b.Genres.Contains(genre))
                .ToList();

            if (books == null)
            {
                return NotFound();
            }

            return Ok(books);
        }
    }
}
