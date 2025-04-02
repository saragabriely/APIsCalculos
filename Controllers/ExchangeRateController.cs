using APIsCalculos.Models.CotacaoDolar;
using APIsCalculos.Servicos;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace APIsCalculos.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExchangeRateController : ControllerBase
    {
        private readonly ILogger<ExchangeRateController> _logger;
        private readonly ControleCotacaoDolar _controleCotacaoDolar;

        public ExchangeRateController(ILogger<ExchangeRateController> logger, ControleCotacaoDolar controleCotacaoDolar)
        {
            _logger = logger;
            _controleCotacaoDolar = controleCotacaoDolar;
        }

        [HttpGet(Name = "Cotação Dólar")]
        public async Task<IActionResult> Get([FromQuery] CotacaoDolarRequest request)
        {
            _logger.LogDebug(">> Iniciando a API de cotação do dólar ... ");

            try
            {
                var retorno = await _controleCotacaoDolar.EnviarRequisicaoCotacaoDolar(request);

                return Ok(retorno);
            }
            catch (Exception e)
            {
                _logger.LogError($"|| Erro ao utilizar a API Cotação Dólar: '{e.Message}'", e);

                return StatusCode(500, new ResponseGenerico
                        {
                            Mensagem = "Erro interno no servidor",
                            Erro = e.Message
                        });
            }
        }
    }
}
