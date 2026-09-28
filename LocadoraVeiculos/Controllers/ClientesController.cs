using LocadoraVeiculos.Data;
using LocadoraVeiculos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LocadoraVeiculos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public ClientesController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/Clientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cliente>>> GetClientes()
        {
            try
            {
                return await _context.Clientes.ToListAsync();
            }
            catch (Exception)
            {
                return StatusCode(
                    500,
                    "Ocorreu um erro ao buscar os clientes."
                );
            }
        }

        // GET: api/Clientes/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Cliente>> GetCliente(int id)
        {
            try
            {
                var cliente = await _context.Clientes.FindAsync(id);

                if (cliente == null)
                {
                    return NotFound("Cliente não encontrado.");
                }

                return cliente;
            }
            catch (Exception)
            {
                return StatusCode(
                    500,
                    "Ocorreu um erro ao buscar o cliente."
                );
            }
        }

        // FILTRO 5 - Clientes por nome com seus aluguéis
        // LEFT JOIN entre Clientes e Alugueis
        // GET: api/Clientes/filtro-alugueis?nome=Maria
        [HttpGet("filtro-alugueis")]
        public async Task<IActionResult> GetClientesComAlugueis(
            [FromQuery] string nome)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nome))
                {
                    return BadRequest(
                        "Informe o nome do cliente para realizar o filtro."
                    );
                }

                var resultado = await (
                    from cliente in _context.Clientes

                    join aluguel in _context.Alugueis
                        on cliente.IdCliente equals aluguel.IdCliente
                        into alugueisCliente

                    from aluguel in alugueisCliente.DefaultIfEmpty()

                    where cliente.Nome.Contains(nome)

                    select new
                    {
                        cliente.IdCliente,
                        cliente.Nome,
                        cliente.CPF,
                        cliente.Email,
                        cliente.Telefone,

                        IdAluguel = aluguel != null
                            ? (int?)aluguel.IdAluguel
                            : null,

                        DataInicio = aluguel != null
                            ? (DateTime?)aluguel.DataInicio
                            : null,

                        DataFim = aluguel != null
                            ? (DateTime?)aluguel.DataFim
                            : null,

                        ValorTotal = aluguel != null
                            ? aluguel.ValorTotal
                            : null
                    }
                ).ToListAsync();

                if (resultado.Count == 0)
                {
                    return NotFound(
                        "Nenhum cliente encontrado com o nome informado."
                    );
                }

                return Ok(resultado);
            }
            catch (Exception)
            {
                return StatusCode(
                    500,
                    "Ocorreu um erro ao filtrar os clientes e seus aluguéis."
                );
            }
        }

        // POST: api/Clientes
        [HttpPost]
        public async Task<ActionResult<Cliente>> PostCliente(Cliente cliente)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var cpfExistente = await _context.Clientes
                    .AnyAsync(c => c.CPF == cliente.CPF);

                if (cpfExistente)
                {
                    return Conflict(
                        "Já existe um cliente cadastrado com este CPF."
                    );
                }

                var emailExistente = await _context.Clientes
                    .AnyAsync(c => c.Email == cliente.Email);

                if (emailExistente)
                {
                    return Conflict(
                        "Já existe um cliente cadastrado com este e-mail."
                    );
                }

                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();

                return CreatedAtAction(
                    nameof(GetCliente),
                    new { id = cliente.IdCliente },
                    cliente
                );
            }
            catch (DbUpdateException)
            {
                return Conflict(
                    "Não foi possível cadastrar o cliente. Verifique se CPF ou e-mail já estão cadastrados."
                );
            }
            catch (Exception)
            {
                return StatusCode(
                    500,
                    "Ocorreu um erro ao cadastrar o cliente."
                );
            }
        }

        // PUT: api/Clientes/1
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCliente(
            int id,
            Cliente cliente)
        {
            if (id != cliente.IdCliente)
            {
                return BadRequest(
                    "O ID informado não corresponde ao cliente."
                );
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var clienteExistente =
                    await _context.Clientes.FindAsync(id);

                if (clienteExistente == null)
                {
                    return NotFound("Cliente não encontrado.");
                }

                var cpfDuplicado = await _context.Clientes
                    .AnyAsync(c =>
                        c.CPF == cliente.CPF &&
                        c.IdCliente != id);

                if (cpfDuplicado)
                {
                    return Conflict(
                        "Já existe outro cliente cadastrado com este CPF."
                    );
                }

                var emailDuplicado = await _context.Clientes
                    .AnyAsync(c =>
                        c.Email == cliente.Email &&
                        c.IdCliente != id);

                if (emailDuplicado)
                {
                    return Conflict(
                        "Já existe outro cliente cadastrado com este e-mail."
                    );
                }

                clienteExistente.Nome = cliente.Nome;
                clienteExistente.CPF = cliente.CPF;
                clienteExistente.Email = cliente.Email;
                clienteExistente.Telefone = cliente.Telefone;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict(
                    "Não foi possível atualizar o cliente devido a um conflito de dados."
                );
            }
            catch (Exception)
            {
                return StatusCode(
                    500,
                    "Ocorreu um erro ao atualizar o cliente."
                );
            }
        }

        // DELETE: api/Clientes/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            try
            {
                var cliente =
                    await _context.Clientes.FindAsync(id);

                if (cliente == null)
                {
                    return NotFound("Cliente não encontrado.");
                }

                _context.Clientes.Remove(cliente);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict(
                    "Não é possível excluir este cliente porque ele possui aluguel relacionado."
                );
            }
            catch (Exception)
            {
                return StatusCode(
                    500,
                    "Ocorreu um erro ao excluir o cliente."
                );
            }
        }
    }
}