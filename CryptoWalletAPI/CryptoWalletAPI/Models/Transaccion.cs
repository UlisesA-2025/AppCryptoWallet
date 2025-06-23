namespace CryptoWalletAPI.Models
{
    public class Transaccion
    {
        public int Id { get; set; }
        public int IdCrypto{ get; set; }
        public int IdExchanges { get; set; }
        public string Tipo { get; set; } 
        public decimal CantCrypto { get; set; }
        public decimal CantARS { get; set; }

        public DateTime Fecha { get; set; }

        public Criptomoneda Criptomoneda { get; set; }
        public Exchange Exchange { get; set; }  
    }
}
