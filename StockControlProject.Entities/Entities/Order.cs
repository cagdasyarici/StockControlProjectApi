using StockControlProject.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockControlProject.Entities.Entities
{
    public class Order:BaseEntity
    {
        public Order()
        {
            OrderDetails = new List<OrderDetails>();
        }
        public Status Status { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public virtual List<OrderDetails> OrderDetails { get; set; }

    }
}
