using LocadoraVeiculos.Data;
using LocadoraVeiculos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LocadoraVeiculos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VeiculosController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public VeiculosController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/Veiculos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Veiculo>>> GetVeiculos()
        {
            try
            {
                var veiculos = await _context.Veiculos
                    .Include(v => v.Fabricante)
                    .Include(v => v.Categoria)
                    .ToListAsync();

                return Ok(veiculos);
            }
            catch (Exception)
            {
                return StatusCode(
                    500,
                    "Ocorreu um erro ao buscar os veículos."
                );
            }
        }

        // GET: api/Veiculos/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Veiculo>> GetVeiculo(int id)
        {
            try
            {
                var veiculo = await _context.Veiculos
                    .Include(v => v.Fabricante)
                    .Include(v => v.Categoria)
                    .FirstOrDefaultAsync(v => v.IdVeiculo == id);

                if (veiculo == null)
                {
                    return NotFound("Veículo não encontrado.");
                }

                return Ok(veiculo);
            }
            catch (Exception)
            {
                return StatusCode(
                    500,
                    "Ocorreu um erro ao buscar o veículo."
                );
            }
        }

        // FILTRO 1 - Veículos por fabricante
        // INNER JOIN entre Veiculos e Fabricantes
        // GET: api/Veiculos/fabricante/1
        [HttpGet("fabricante/{idFabricante}")]
        public async Task<IActionResult> GetVeiculosPorFabricante(
            int idFabricante)
        {
            try
            {
                var resultado = await (
                    from veiculo in _context.Veiculos
                    join fabricante in _context.Fabricantes
                        on veiculo.IdFabricante equals fabricante.IdFabricante
                    where fabricante.IdFabricante == idFabricante
                    select new
                    {
                        veiculo.IdVeiculo,
                        veiculo.Modelo,
                        veiculo.AnoFabricacao,
                        veiculo.Quilometragem,
                        veiculo.Placa,
                        Fabricante = fabricante.Nome
                    }
                ).ToListAsync();

                if (resultado.Count == 0)
                {
                    return NotFound(
                        "Nenhum veículo encontrado para este fabricante."
                    );
                }

                return Ok(resultado);
            }
            catch (Exception)
            {
                return StatusCode(
                    500,
                    "Ocorreu um erro ao filtrar os veículos por fabricante."
                );
            }
        }

        // FILTRO 2 - Veículos por categoria
// INNER JOIN entre Veiculos e Categorias
// GET: api/Veiculos/categoria/1
[HttpGet("categoria/{idCategoria}")]
public async Task<IActionResult> GetVeiculosPorCategoria(
    int idCategoria)
{
    try
    {
        var resultado = await (
            from veiculo in _context.Veiculos
            join categoria in _context.Categorias
                on veiculo.IdCategoria equals categoria.IdCategoria
            where categoria.IdCategoria == idCategoria
            select new
            {
                veiculo.IdVeiculo,
                veiculo.Modelo,
                veiculo.AnoFabricacao,
                veiculo.Quilometragem,
                veiculo.Placa,
                Categoria = categoria.Nome
            }
        ).ToListAsync();

        if (resultado.Count == 0)
        {
            return NotFound(
                "Nenhum veículo encontrado para esta categoria."
            );
        }

        return Ok(resultado);
    }
    catch (Exception)
    {
        return StatusCode(
            500,
            "Ocorreu um erro ao filtrar os veículos por categoria."
        );
    }
}

        // POST: api/Veiculos
        [HttpPost]
        public async Task<ActionResult<Veiculo>> PostVeiculo(Veiculo veiculo)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var placaExistente = await _context.Veiculos
                    .AnyAsync(v => v.Placa == veiculo.Placa);

                if (placaExistente)
                {
                    return Conflict(
                        "Já existe um veículo cadastrado com esta placa."
                    );
                }

                var fabricanteExiste = await _context.Fabricantes
                    .AnyAsync(f => f.IdFabricante == veiculo.IdFabricante);

                if (!fabricanteExiste)
                {
                    return BadRequest(
                        "O fabricante informado não existe."
                    );
                }

                var categoriaExiste = await _context.Categorias
                    .AnyAsync(c => c.IdCategoria == veiculo.IdCategoria);

                if (!categoriaExiste)
                {
                    return BadRequest(
                        "A categoria informada não existe."
                    );
                }

                // Evita que objetos de navegação enviados no JSON
                // sejam cadastrados novamente.
                veiculo.Fabricante = null;
                veiculo.Categoria = null;

                _context.Veiculos.Add(veiculo);
                await _context.SaveChangesAsync();

                return CreatedAtAction(
                    nameof(GetVeiculo),
                    new { id = veiculo.IdVeiculo },
                    veiculo
                );
            }
            catch (DbUpdateException)
            {
                return Conflict(
                    "Não foi possível cadastrar o veículo devido a um conflito de dados."
                );
            }
            catch (Exception)
            {
                return StatusCode(
                    500,
                    "Ocorreu um erro ao cadastrar o veículo."
                );
            }
        }

        // PUT: api/Veiculos/1
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVeiculo(
            int id,
            Veiculo veiculo)
        {
            if (id != veiculo.IdVeiculo)
            {
                return BadRequest(
                    "O ID informado não corresponde ao veículo."
                );
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var veiculoExistente =
                    await _context.Veiculos.FindAsync(id);

                if (veiculoExistente == null)
                {
                    return NotFound("Veículo não encontrado.");
                }

                var placaDuplicada = await _context.Veiculos
                    .AnyAsync(v =>
                        v.Placa == veiculo.Placa &&
                        v.IdVeiculo != id);

                if (placaDuplicada)
                {
                    return Conflict(
                        "Já existe outro veículo cadastrado com esta placa."
                    );
                }

                var fabricanteExiste = await _context.Fabricantes
                    .AnyAsync(f =>
                        f.IdFabricante == veiculo.IdFabricante);

                if (!fabricanteExiste)
                {
                    return BadRequest(
                        "O fabricante informado não existe."
                    );
                }

                var categoriaExiste = await _context.Categorias
                    .AnyAsync(c =>
                        c.IdCategoria == veiculo.IdCategoria);

                if (!categoriaExiste)
                {
                    return BadRequest(
                        "A categoria informada não existe."
                    );
                }

                veiculoExistente.Modelo = veiculo.Modelo;
                veiculoExistente.AnoFabricacao = veiculo.AnoFabricacao;
                veiculoExistente.Quilometragem = veiculo.Quilometragem;
                veiculoExistente.Placa = veiculo.Placa;
                veiculoExistente.IdFabricante = veiculo.IdFabricante;
                veiculoExistente.IdCategoria = veiculo.IdCategoria;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict(
                    "Não foi possível atualizar o veículo devido a um conflito de dados."
                );
            }
            catch (Exception)
            {
                return StatusCode(
                    500,
                    "Ocorreu um erro ao atualizar o veículo."
                );
            }
        }

        // DELETE: api/Veiculos/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVeiculo(int id)
        {
            try
            {
                var veiculo =
                    await _context.Veiculos.FindAsync(id);

                if (veiculo == null)
                {
                    return NotFound("Veículo não encontrado.");
                }

                _context.Veiculos.Remove(veiculo);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict(
                    "Não é possível excluir este veículo porque ele possui aluguel relacionado."
                );
            }
            catch (Exception)
            {
                return StatusCode(
                    500,
                    "Ocorreu um erro ao excluir o veículo."
                );
            }
        }
    }
}