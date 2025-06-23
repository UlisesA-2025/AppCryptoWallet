namespace CryptoWalletAPI.Models.Dtos
{
    public class CriptoYaRespuesta
    {
        public decimal? ask { get; set; }
        public decimal? totalAsk { get; set; }
        public decimal? bid { get; set; }
        public decimal? totalBid { get; set; }
        public long time { get; set; }
    }
}
