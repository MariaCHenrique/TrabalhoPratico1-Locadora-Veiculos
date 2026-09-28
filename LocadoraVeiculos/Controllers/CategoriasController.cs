using LocadoraVeiculos.Data;
using LocadoraVeiculos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LocadoraVeiculos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriasController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public CategoriasController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/Categorias
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categoria>>> GetCategorias()
        {
            try
            {
                return await _context.Categorias.ToListAsync();
            }
            catch (Exception)
            {
                return StatusCode(
                    500,
                    "Ocorreu um erro ao buscar as categorias."
                );
            }
        }

        // GET: api/Categorias/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Categoria>> GetCategoria(int id)
        {
            try
            {
                var categoria = await _context.Categorias.FindAsync(id);

                if (categoria == null)
                {
                    return NotFound("Categoria não encontrada.");
                }

                return categoria;
            }
            catch (Exception)
            {
                return StatusCode(
                    500,
                    "Ocorreu um erro ao buscar a categoria."
                );
            }
        }

        // POST: api/Categorias
        [HttpPost]
        public async Task<ActionResult<Categoria>> PostCategoria(
            Categoria categoria)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _context.Categorias.Add(categoria);
                await _context.SaveChangesAsync();

                return CreatedAtAction(
                    nameof(GetCategoria),
                    new { id = categoria.IdCategoria },
                    categoria
                );
            }
            catch (Exception)
            {
                return StatusCode(
                    500,
                    "Ocorreu um erro ao cadastrar a categoria."
                );
            }
        }

        // PUT: api/Categorias/1
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategoria(
            int id,
            Categoria categoria)
        {
            if (id != categoria.IdCategoria)
            {
                return BadRequest(
                    "O ID informado não corresponde à categoria."
                );
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var categoriaExistente =
                    await _context.Categorias.FindAsync(id);

                if (categoriaExistente == null)
                {
                    return NotFound("Categoria não encontrada.");
                }

                categoriaExistente.Nome = categoria.Nome;
                categoriaExistente.Descricao = categoria.Descricao;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(
                    500,
                    "Ocorreu um erro ao atualizar a categoria."
                );
            }
        }

        // DELETE: api/Categorias/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoria(int id)
        {
            try
            {
                var categoria =
                    await _context.Categorias.FindAsync(id);

                if (categoria == null)
                {
                    return NotFound("Categoria não encontrada.");
                }

                _context.Categorias.Remove(categoria);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict(
                    "Não é possível excluir esta categoria porque ela está relacionada a um veículo."
                );
            }
            catch (Exception)
            {
                return StatusCode(
                    500,
                    "Ocorreu um erro ao excluir a categoria."
                );
            }
        }
    }
}