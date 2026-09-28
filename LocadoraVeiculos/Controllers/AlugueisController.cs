using LocadoraVeiculos.Data;
using LocadoraVeiculos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LocadoraVeiculos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlugueisController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public AlugueisController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/Alugueis
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Aluguel>>> GetAlugueis()
        {
            try
            {
                var alugueis = await _context.Alugueis
                    .Include(a => a.Cliente)
                    .Include(a => a.Veiculo)
                        .ThenInclude(v => v!.Fabricante)
                    .Include(a => a.Veiculo)
                        .ThenInclude(v => v!.Categoria)
                    .ToListAsync();

                return Ok(alugueis);
            }
            catch (Exception)
            {
                return StatusCode(
                    500,
                    "Ocorreu um erro ao buscar os aluguéis."
                );
            }
        }

        // GET: api/Alugueis/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Aluguel>> GetAluguel(int id)
        {
            try
            {
                var aluguel = await _context.Alugueis
                    .Include(a => a.Cliente)
                    .Include(a => a.Veiculo)
                        .ThenInclude(v => v!.Fabricante)
                    .Include(a => a.Veiculo)
                        .ThenInclude(v => v!.Categoria)
                    .FirstOrDefaultAsync(a => a.IdAluguel == id);

                if (aluguel == null)
                {
                    return NotFound("Aluguel não encontrado.");
                }

                return Ok(aluguel);
            }
            catch (Exception)
            {
                return StatusCode(
                    500,
                    "Ocorreu um erro ao buscar o aluguel."
                );
            }
        }

        // FILTRO 3 - Aluguéis por cliente
        // INNER JOIN entre Alugueis e Clientes
        // GET: api/Alugueis/cliente/1
        [HttpGet("cliente/{idCliente}")]
        public async Task<IActionResult> GetAlugueisPorCliente(
            int idCliente)
        {
            try
            {
                var resultado = await (
                    from aluguel in _context.Alugueis
                    join cliente in _context.Clientes
                        on aluguel.IdCliente equals cliente.IdCliente
                    where cliente.IdCliente == idCliente
                    select new
                    {
                        aluguel.IdAluguel,
                        Cliente = cliente.Nome,
                        cliente.CPF,
                        aluguel.IdVeiculo,
                        aluguel.DataInicio,
                        aluguel.DataFim,
                        aluguel.DataDevolucao,
                        aluguel.QuilometragemInicial,
                        aluguel.QuilometragemFinal,
                        aluguel.ValorDiaria,
                        aluguel.ValorTotal
                    }
                ).ToListAsync();

                if (resultado.Count == 0)
                {
                    return NotFound(
                        "Nenhum aluguel encontrado para este cliente."
                    );
                }

                return Ok(resultado);
            }
            catch (Exception)
            {
                return StatusCode(
                    500,
                    "Ocorreu um erro ao filtrar os aluguéis por cliente."
                );
            }
        }

        // FILTRO 4 - Aluguéis por veículo
// INNER JOIN entre Alugueis e Veiculos
// GET: api/Alugueis/veiculo/1
[HttpGet("veiculo/{idVeiculo}")]
public async Task<IActionResult> GetAlugueisPorVeiculo(
    int idVeiculo)
{
    try
    {
        var resultado = await (
            from aluguel in _context.Alugueis
            join veiculo in _context.Veiculos
                on aluguel.IdVeiculo equals veiculo.IdVeiculo
            where veiculo.IdVeiculo == idVeiculo
            select new
            {
                aluguel.IdAluguel,
                veiculo.IdVeiculo,
                veiculo.Modelo,
                veiculo.Placa,
                aluguel.IdCliente,
                aluguel.DataInicio,
                aluguel.DataFim,
                aluguel.DataDevolucao,
                aluguel.QuilometragemInicial,
                aluguel.QuilometragemFinal,
                aluguel.ValorDiaria,
                aluguel.ValorTotal
            }
        ).ToListAsync();

        if (resultado.Count == 0)
        {
            return NotFound(
                "Nenhum aluguel encontrado para este veículo."
            );
        }

        return Ok(resultado);
    }
    catch (Exception)
    {
        return StatusCode(
            500,
            "Ocorreu um erro ao filtrar os aluguéis por veículo."
        );
    }
}

        // POST: api/Alugueis
        [HttpPost]
        public async Task<ActionResult<Aluguel>> PostAluguel(Aluguel aluguel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var clienteExiste = await _context.Clientes
                    .AnyAsync(c => c.IdCliente == aluguel.IdCliente);

                if (!clienteExiste)
                {
                    return BadRequest(
                        "O cliente informado não existe."
                    );
                }

                var veiculoExiste = await _context.Veiculos
                    .AnyAsync(v => v.IdVeiculo == aluguel.IdVeiculo);

                if (!veiculoExiste)
                {
                    return BadRequest(
                        "O veículo informado não existe."
                    );
                }

                if (aluguel.DataFim < aluguel.DataInicio)
                {
                    return BadRequest(
                        "A data final não pode ser anterior à data inicial."
                    );
                }

                if (aluguel.QuilometragemInicial < 0)
                {
                    return BadRequest(
                        "A quilometragem inicial não pode ser negativa."
                    );
                }

                if (aluguel.ValorDiaria <= 0)
                {
                    return BadRequest(
                        "O valor da diária deve ser maior que zero."
                    );
                }

                if (aluguel.DataDevolucao.HasValue &&
                    aluguel.DataDevolucao.Value < aluguel.DataInicio)
                {
                    return BadRequest(
                        "A data de devolução não pode ser anterior à data inicial."
                    );
                }

                if (aluguel.QuilometragemFinal.HasValue &&
                    aluguel.QuilometragemFinal.Value <
                    aluguel.QuilometragemInicial)
                {
                    return BadRequest(
                        "A quilometragem final não pode ser menor que a inicial."
                    );
                }

                aluguel.Cliente = null;
                aluguel.Veiculo = null;

                _context.Alugueis.Add(aluguel);
                await _context.SaveChangesAsync();

                return CreatedAtAction(
                    nameof(GetAluguel),
                    new { id = aluguel.IdAluguel },
                    aluguel
                );
            }
            catch (DbUpdateException)
            {
                return Conflict(
                    "Não foi possível cadastrar o aluguel devido a um conflito de dados."
                );
            }
            catch (Exception)
            {
                return StatusCode(
                    500,
                    "Ocorreu um erro ao cadastrar o aluguel."
                );
            }
        }

        // PUT: api/Alugueis/1
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAluguel(
            int id,
            Aluguel aluguel)
        {
            if (id != aluguel.IdAluguel)
            {
                return BadRequest(
                    "O ID informado não corresponde ao aluguel."
                );
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var aluguelExistente =
                    await _context.Alugueis.FindAsync(id);

                if (aluguelExistente == null)
                {
                    return NotFound("Aluguel não encontrado.");
                }

                var clienteExiste = await _context.Clientes
                    .AnyAsync(c => c.IdCliente == aluguel.IdCliente);

                if (!clienteExiste)
                {
                    return BadRequest(
                        "O cliente informado não existe."
                    );
                }

                var veiculoExiste = await _context.Veiculos
                    .AnyAsync(v => v.IdVeiculo == aluguel.IdVeiculo);

                if (!veiculoExiste)
                {
                    return BadRequest(
                        "O veículo informado não existe."
                    );
                }

                if (aluguel.DataFim < aluguel.DataInicio)
                {
                    return BadRequest(
                        "A data final não pode ser anterior à data inicial."
                    );
                }

                if (aluguel.QuilometragemInicial < 0)
                {
                    return BadRequest(
                        "A quilometragem inicial não pode ser negativa."
                    );
                }

                if (aluguel.ValorDiaria <= 0)
                {
                    return BadRequest(
                        "O valor da diária deve ser maior que zero."
                    );
                }

                if (aluguel.DataDevolucao.HasValue &&
                    aluguel.DataDevolucao.Value < aluguel.DataInicio)
                {
                    return BadRequest(
                        "A data de devolução não pode ser anterior à data inicial."
                    );
                }

                if (aluguel.QuilometragemFinal.HasValue &&
                    aluguel.QuilometragemFinal.Value <
                    aluguel.QuilometragemInicial)
                {
                    return BadRequest(
                        "A quilometragem final não pode ser menor que a inicial."
                    );
                }

                aluguelExistente.IdCliente = aluguel.IdCliente;
                aluguelExistente.IdVeiculo = aluguel.IdVeiculo;
                aluguelExistente.DataInicio = aluguel.DataInicio;
                aluguelExistente.DataFim = aluguel.DataFim;
                aluguelExistente.DataDevolucao = aluguel.DataDevolucao;
                aluguelExistente.QuilometragemInicial =
                    aluguel.QuilometragemInicial;
                aluguelExistente.QuilometragemFinal =
                    aluguel.QuilometragemFinal;
                aluguelExistente.ValorDiaria = aluguel.ValorDiaria;
                aluguelExistente.ValorTotal = aluguel.ValorTotal;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict(
                    "Não foi possível atualizar o aluguel devido a um conflito de dados."
                );
            }
            catch (Exception)
            {
                return StatusCode(
                    500,
                    "Ocorreu um erro ao atualizar o aluguel."
                );
            }
        }

        // DELETE: api/Alugueis/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAluguel(int id)
        {
            try
            {
                var aluguel =
                    await _context.Alugueis.FindAsync(id);

                if (aluguel == null)
                {
                    return NotFound("Aluguel não encontrado.");
                }

                _context.Alugueis.Remove(aluguel);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(
                    500,
                    "Ocorreu um erro ao excluir o aluguel."
                );
            }
        }
    }
}