using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace INVUIs.Purchases.PurchaseModels
{
    public class PurchaseProductModel
    {
        public Guid Id { get; set; }

        public Guid PurchaseOrderId { get; set; }

        public Guid WareHouseId { get; set; }

        public string? Designation { get; set; }

        public string? UnitMeasure { get; set; }

        public int Quantity { get; set; }

        public int TVA { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TotalPrice { get; set; }
        /*  ID = product.ID,
            IDPurchaseOrder = product.IDPurchaseOrder,
            Designation = product.Designation,
            UnitMeasure = product.UnitMeasure,
            Quantity = product.Quantity,
            UnitPrice = product.UnitPrice,
            TVA = product.TVA*/
        public int Received { get; set; }
    }
}