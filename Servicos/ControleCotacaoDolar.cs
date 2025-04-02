using APIsCalculos.Controllers;
using APIsCalculos.Models.CotacaoDolar;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace APIsCalculos.Servicos
{
    public class ControleCotacaoDolar
    {
        private readonly string link    = "https://v6.exchangerate-api.com/v6/";
        private readonly string token   = "13c12344ad07291a4d75c197";
        public static HttpClient client = new HttpClient();
        private readonly ILogger<ExchangeRateController> _logger;

        public ControleCotacaoDolar(ILogger<ExchangeRateController> logger)
        {
            _logger = logger;
        }

        public async Task<ResponseGenerico> EnviarRequisicaoCotacaoDolar(CotacaoDolarRequest request)
        {
            try
            {
                var responseGenerico = new ResponseGenerico();
                var valorConvertido  = 0m;

                string url = BuscarURLCotacaoDolar();

                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return responseGenerico;

                string jsonResponse = await response.Content.ReadAsStringAsync();

                var resultado = JsonSerializer.Deserialize<ResponseAPICotacaoDolar>(jsonResponse);

                if(resultado == null || !resultado.result.Equals("success"))
                    return responseGenerico;

                decimal cotacaoDolar = resultado.conversion_rates["BRL"];

                if (cotacaoDolar < 0)
                    return responseGenerico;

                valorConvertido = CalcularCambio(request.ValorEmReal, cotacaoDolar);

                responseGenerico = new ResponseGenerico { Mensagem = $"$ {valorConvertido.ToString("F2")} USD" };

                return responseGenerico;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro na requisição de cotação: {ex.Message}", ex);
                return new ResponseGenerico { Mensagem = "", Erro = $"Erro na requisição de cotação: '{ex.Message}'" }; 
            }
        }

        private decimal CalcularCambio(decimal valorEmReal, decimal cotacaoDolar)
        {
            return valorEmReal / cotacaoDolar;
        }

        private string BuscarURLCotacaoDolar()
        {
            return $"{link}{token}/latest/USD";
        }
    }
}
