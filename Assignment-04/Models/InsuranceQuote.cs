using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Assignment_04.Models
{
    public class InsuranceQuote
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public decimal MonthlyQuote { get; set; }

        // Navigation property
        public virtual Customer Customer { get; set; }
    }

}
