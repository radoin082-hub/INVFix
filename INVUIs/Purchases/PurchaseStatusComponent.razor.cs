using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INV.Domain.Entities.Purchases;
using INV.Domain.Entities.Receipts;
using Microsoft.AspNetCore.Components;

namespace INVUIs.Purchases
{
    public partial class PurchaseStatusComponent
    {
        [Parameter] public PurchaseStatus PurchaseStatus { get; set; }
        [Parameter] public bool icon { get; set; }
        [Parameter] public bool text { get; set; }

        private string GetStatusClass()
        {
            return PurchaseStatus switch
            {
                PurchaseStatus.Vised => "status-completed",
                PurchaseStatus.Editing => "status-in-progress",
                PurchaseStatus.Reject => "status-canceled",
                PurchaseStatus.Validated => "status-pending",
                _ => string.Empty
            };
        }

        private string GetStatusIcon()
        {
            return PurchaseStatus switch
            {
                PurchaseStatus.Vised => "bi bi-check-circle", // Font Awesome icon for completed
                PurchaseStatus.Editing => "bi bi-pen", // Font Awesome spinning icon
                PurchaseStatus.Reject => "bi bi-x-circle", // Font Awesome clock icon
                PurchaseStatus.Validated => "bi bi-hourglass-split", // Font Awesome clock icon
                _ => "fas fa-question-circle"
            };
        }
    }
}