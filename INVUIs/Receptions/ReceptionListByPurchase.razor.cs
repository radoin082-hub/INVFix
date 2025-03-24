using INV.Domain.Entities.Receipts;
using Microsoft.AspNetCore.Components;

namespace INVUIs.Receptions;

public partial class ReceptionListByPurchase
{
    [Parameter] public List<Receipt> Receptions { get; set; }
    [Parameter] public RenderFragment Pills { get; set; }
}