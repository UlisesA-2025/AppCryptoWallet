using CryptoWalletAPI.Data;
using CryptoWalletAPI.Models;
using CryptoWalletAPI.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;

namespace CryptoWalletAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransaccionesController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly HttpClient _httpClient;

        public TransaccionesController(DataContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }

        private async Task<decimal> ObtenerPrecio(string codigoCripto, string exchange, string tipoTransaccion)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<CriptoYaRespuesta>(
                    $"https://criptoya.com/api/{exchange}/{codigoCripto}/ars");

                if (response == null)
                    return 0;

                if (tipoTransaccion.ToLower() == "purchase")
                    return response.totalAsk ?? response.ask ?? 0;
                else if (tipoTransaccion.ToLower() == "sale")
                    return response.totalBid ?? response.bid ?? 0;
                else
                    return 0;
            }
            catch
            {
                return 0;
            }
        }


        [HttpPost]
        public async Task<IActionResult> CrearTransaccion([FromBody] TransaccionDTO dto)
        {
            if (dto.CantCrypto <= 0)
                return BadRequest("Cantidad inválida.");

            var crypto = await _context.Criptomonedas.FirstOrDefaultAsync(c => c.Crypto_Code == dto.Crypto_Code);
            if (crypto == null)
                return NotFound("Criptomoneda no encontrada.");

            var exchange = await _context.Exchanges.FirstOrDefaultAsync();
            if (exchange == null)
                return NotFound("Exchange no encontrado.");

            decimal precio = await ObtenerPrecio(dto.Crypto_Code, dto.ExchangeNombre, dto.Tipo);
            decimal totalARS = dto.CantCrypto * precio;

            if (dto.Tipo == "sale")
            {
                decimal totalComprado = await _context.Transacciones
                    .Where(t => t.IdCrypto == crypto.Id && t.Tipo == "purchase")
                    .SumAsync(t => t.CantCrypto);

                decimal totalVendido = await _context.Transacciones
                    .Where(t => t.IdCrypto == crypto.Id && t.Tipo == "sale")
                    .SumAsync(t => t.CantCrypto);

                if ((totalComprado - totalVendido) < dto.CantCrypto)
                    return BadRequest("No tenés suficiente saldo para vender.");
            }

            var transaccion = new Transaccion
            {
                IdCrypto = crypto.Id,
                IdExchanges = exchange.Id,
                Tipo = dto.Tipo,
                CantCrypto = dto.CantCrypto,
                CantARS = totalARS,
                Fecha = dto.Fecha
            };

            _context.Transacciones.Add(transaccion);
            await _context.SaveChangesAsync();

            return Ok("Transacción guardada correctamente.");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransaccionRespuestaDTO>>> ObtenerTransacciones()
        {
            var transacciones = await _context.Transacciones
                .Include(t => t.Criptomoneda)
                .Include(t => t.Exchange)
                .OrderByDescending(t => t.Fecha)
                .ToListAsync();

            var respuesta = transacciones.Select(t => new TransaccionRespuestaDTO
            {
                Id = t.Id,
                Crypto_Code = t.Criptomoneda.Crypto_Code,
                CriptomonedaNombre = t.Criptomoneda.Nombre,
                Tipo = t.Tipo,
                CantCrypto = t.CantCrypto,
                CantARS = t.CantARS,
                Fecha = t.Fecha,
                ExchangeNombre = t.Exchange.Nombre
            });

            return Ok(respuesta);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TransaccionRespuestaDTO>> ObtenerTransaccion(int id)
        {
            var t = await _context.Transacciones
                .Include(t => t.Criptomoneda)
                .Include(t => t.Exchange)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (t == null) return NotFound();

            return Ok(new TransaccionRespuestaDTO
            {
                Id = t.Id,
                Crypto_Code = t.Criptomoneda.Crypto_Code,
                CriptomonedaNombre = t.Criptomoneda.Nombre,
                Tipo = t.Tipo,
                CantCrypto = t.CantCrypto,
                CantARS = t.CantARS,
                Fecha = t.Fecha,
                ExchangeNombre = t.Exchange.Nombre
            });
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> EditarTransaccion(int id, [FromBody] TransaccionDTO dto)
        {
            var t = await _context.Transacciones.FindAsync(id);
            if (t == null) return NotFound();

            var crypto = await _context.Criptomonedas.FirstOrDefaultAsync(c => c.Crypto_Code == dto.Crypto_Code);
            if (crypto == null) return NotFound("Criptomoneda no encontrada.");

            var exchange = await _context.Exchanges.FirstOrDefaultAsync(e => e.Nombre == dto.ExchangeNombre);
            if (exchange == null) return NotFound("Exchange no encontrado.");

            decimal precio = await ObtenerPrecio(dto.Crypto_Code, dto.ExchangeNombre, dto.Tipo);
            if (precio == 0) return BadRequest("No se pudo obtener el precio actual.");

            t.Tipo = dto.Tipo;
            t.CantCrypto = dto.CantCrypto;
            t.Fecha = dto.Fecha;
            t.CantARS = dto.CantCrypto * precio;

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Transacción actualizada correctamente." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarTransaccion(int id)
        {
            var t = await _context.Transacciones.FindAsync(id);
            if (t == null) return NotFound();

            _context.Transacciones.Remove(t);
            await _context.SaveChangesAsync();
            return Ok("Transacción eliminada.");
        }

        [HttpGet("estado")]
        public async Task<IActionResult> ObtenerEstado()
        {
            var cryptos = await _context.Criptomonedas.ToListAsync();
            var resultado = new List<object>();
            decimal total = 0;

            foreach (var c in cryptos)
            {
                decimal comprados = await _context.Transacciones
                    .Where(t => t.CriptomonedaId == c.Id && t.Tipo == "purchase").SumAsync(t => t.CantCrypto);

                decimal vendidos = await _context.Transacciones
                    .Where(t => t.CriptomonedaId == c.Id && t.Tipo == "sale").SumAsync(t => t.CantCrypto);

                decimal tenencia = comprados - vendidos;

                if (tenencia > 0)
                {
                    decimal precio = await ObtenerPrecio(c.Crypto_Code, c.Nombre, "sale");
                    decimal valorARS = tenencia * precio;
                    total += valorARS;

                    resultado.Add(new
                    {
                        Criptomoneda = c.Nombre,
                        Cantidad = tenencia,
                        ValorEnARS = Math.Round(valorARS, 2)
                    });
                }
            }

            return Ok(new { Estado = resultado, Total = Math.Round(total, 2) });
        }


    }
}
