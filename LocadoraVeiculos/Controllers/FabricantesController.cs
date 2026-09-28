using LocadoraVeiculos.Data;
using LocadoraVeiculos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LocadoraVeiculos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FabricantesController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public FabricantesController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/Fabricantes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Fabricante>>> GetFabricantes()
        {
            try
            {
                return await _context.Fabricantes.ToListAsync();
            }
            catch (Exception)
            {
                return StatusCode(
                    500,
                    "Ocorreu um erro ao buscar os fabricantes."
                );
            }
        }

        // GET: api/Fabricantes/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Fabricante>> GetFabricante(int id)
        {
            try
            {
                var fabricante = await _context.Fabricantes.FindAsync(id);

                if (fabricante == null)
                {
                    return NotFound("Fabricante não encontrado.");
                }

                return fabricante;
            }
            catch (Exception)
            {
                return StatusCode(
                    500,
                    "Ocorreu um erro ao buscar o fabricante."
                );
            }
        }

        // POST: api/Fabricantes
        [HttpPost]
        public async Task<ActionResult<Fabricante>> PostFabricante(
            Fabricante fabricante)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _context.Fabricantes.Add(fabricante);
                await _context.SaveChangesAsync();

                return CreatedAtAction(
                    nameof(GetFabricante),
                    new { id = fabricante.IdFabricante },
                    fabricante
                );
            }
            catch (Exception)
            {
                return StatusCode(
                    500,
                    "Ocorreu um erro ao cadastrar o fabricante."
                );
            }
        }

        // PUT: api/Fabricantes/1
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFabricante(
            int id,
            Fabricante fabricante)
        {
            if (id != fabricante.IdFabricante)
            {
                return BadRequest(
                    "O ID informado não corresponde ao fabricante."
                );
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var fabricanteExistente =
                    await _context.Fabricantes.FindAsync(id);

                if (fabricanteExistente == null)
                {
                    return NotFound("Fabricante não encontrado.");
                }

                fabricanteExistente.Nome = fabricante.Nome;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(
                    500,
                    "Ocorreu um erro ao atualizar o fabricante."
                );
            }
        }

        // DELETE: api/Fabricantes/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFabricante(int id)
        {
            try
            {
                var fabricante =
                    await _context.Fabricantes.FindAsync(id);

                if (fabricante == null)
                {
                    return NotFound("Fabricante não encontrado.");
                }

                _context.Fabricantes.Remove(fabricante);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict(
                    "Não é possível excluir este fabricante porque ele está relacionado a um veículo."
                );
            }
            catch (Exception)
            {
                return StatusCode(
                    500,
                    "Ocorreu um erro ao excluir o fabricante."
                );
            }
        }
    }
}