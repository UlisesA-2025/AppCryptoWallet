namespace CryptoWalletAPI.Models.Dtos
{
    public class TransaccionRespuestaDTO
    {
        public int Id { get; set; }
        public string Crypto_Code { get; set; }
        public string CriptomonedaNombre { get; set; }
        public string Tipo { get; set; }
        public decimal CantCrypto { get; set; }
        public decimal CantARS { get; set; }
        public DateTime Fecha { get; set; }
        public string ExchangeNombre { get; set; }
    }
}
