using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INV.App.Purchases;
using INV.Domain.Entities.Purchases;
using INV.Domain.Entities.Receipts;

namespace INV.App.Receipts
{
    public class ReceiptDetail
    {
        public Guid Id { get; set; }
        public string Number { get; set; }
        public DateOnly? Date { get; set; }
        public Guid PurchaseId { get; set; }
        public string purchaseNumber { get; set; }
        public DateOnly PurchaseDate { get; set; }
        public Guid supplierId { get; set; }
        public string supplierName { get; set; }
        public string DeliveryNumber { get; set; }
        public DateOnly? DeliveryDate { get; set; }
        public ReceiptStatus Status { get; set; }
        
        public PurchaseOrderInfo PurchaseOrder { get; set; } = new ();
        public List<ReceiptProductInfo> ReceiptProducts { get; set; }
    }
}