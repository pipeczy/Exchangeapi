using System.ComponentModel.DataAnnotations.Schema;

namespace Exchangeapi.Models
{
    public class ExchangeRate
    {
        public int Id { get; set; }
        public string BaseCurrency { get; set; } = null!;
        public string TargetCurrency { get; set; } = null!;
        public decimal Rate { get; set; }
        public DateTime Date { get; set; }

    }
}
