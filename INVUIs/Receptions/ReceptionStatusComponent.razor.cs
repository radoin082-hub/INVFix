using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INV.Domain.Entities.Purchases;
using INV.Domain.Entities.Receipts;
using Microsoft.AspNetCore.Components;

namespace INVUIs.Receptions
{
    public partial class ReceptionStatusComponent
    {
        [Parameter] public ReceiptStatus ReceptionStatus { get; set; }

        [Parameter] public bool icon { get; set; }
        [Parameter] public bool text { get; set; }

        private string GetStatusClass()
        {
            return ReceptionStatus switch
            {
                ReceiptStatus.validated => "status-completed",
                ReceiptStatus.editing => "status-in-progress",

                _ => string.Empty
            };
        }

        private string GetStatusIcon()
        {
            return ReceptionStatus switch
            {
                ReceiptStatus.validated => "bi bi-check-circle", // Font Awesome icon for completed
                ReceiptStatus.editing => "bi bi-pen", // Font Awesome spinning icon
                _ => "fas fa-question-circle"
            };
        }
    }
}