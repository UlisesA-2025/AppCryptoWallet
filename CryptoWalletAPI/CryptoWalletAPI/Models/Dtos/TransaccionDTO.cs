namespace CryptoWalletAPI.Models.Dtos
{
    public class TransaccionDTO
    {
        public string Crypto_Code { get; set; } 
        public string Tipo { get; set; }
        public string ExchangeNombre { get; set; }
        public decimal CantCrypto { get; set; }
        public DateTime Fecha { get; set; }
    }
}
