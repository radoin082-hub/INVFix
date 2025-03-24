using System.ComponentModel.DataAnnotations;
using INV.App.Budgets;
using INV.Domain.Entities.Budget;
using INV.Domain.Entities.Purchases;
using INVUIs.Purchases.PurchaseModels;
using INVUIs.Shared;
using INVUIs.Shared.MyAlert;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace INVUIs.Purchases;

public partial class PurchaseHeader : ComponentBase
{
    [CascadingParameter] public List<Chapter?> Chapters { get; set; }
    [CascadingParameter] public List<Article?> Articles { get; set; }
    [Parameter] public PurchaseModel Purchase { get; set; } = new();
    [Parameter] public EventCallback OnCreate { get; set; }
    [Parameter] public EventCallback<PurchaseModel> OnPurchaseOrder { get; set; }
    [Inject] private IBudgetService budgetService { get; set; }
    [Inject] private IJSRuntime jsRuntime { set; get; }
    private MyAlert myAlert { set; get; }

    private int _selectedChapterCode;
    private int _selelctedArticleCode;
    private int selectedChapterCode;
    private EditForm form;
    private bool displayVisa = false;
    private bool displayReject = false;

    public int SelectedArticleCode
    {
        get => Purchase.selectedArticleId;
        set
        {
            if (_selelctedArticleCode != value)
            {
                _selelctedArticleCode = value;
                Purchase.selectedArticle = value.ToString();
                LoadArticleTitle();
            }
        }
    }

    public int SelectedChapterCode
    {
        get => Purchase.selectedChapterId;
        set
        {
            if (_selectedChapterCode != value)
            {
                _selectedChapterCode = value;
                Purchase.selectedChapter = value.ToString();
                LoadChapterTitle();
                LoadArticlesBycodeChapter();
            }
        }
    }

    private async Task create()
    {
        await OnCreate.InvokeAsync();
    }

    /*protected override void OnInitialized()
    {
        base.OnInitialized();
    }*/

    protected override async Task OnInitializedAsync()
    {
        myAlert = new MyAlert(jsRuntime);
        /* var result = await budgetService.GetAllChapitres();
         if (result.IsSuccess)
         {
             chapters = result.Value;
         }*/
    }

    private async void LoadChapterTitle()
    {
        var result = await budgetService.GetChapterByCode(SelectedChapterCode);

        Purchase.title_chapter = result.Value.Name;
        StateHasChanged();
    }

    private async void LoadArticlesBycodeChapter()
    {
        var result = await budgetService.GetArticlesByCodeChapter(SelectedChapterCode);
        Articles = result.Value;

        StateHasChanged();
    }

    private async void LoadArticleTitle()
    {
        var result = await budgetService.GetArticlesByCodeArticle(SelectedArticleCode);

        Purchase.description_article = result.Value.Name;
        StateHasChanged();
    }

    public async Task Save()
    {
        await OnPurchaseOrder.InvokeAsync(Purchase);
    }

    public async Task SubmitForm()
    {
        if (form is not null)
        {
            var editContext = form.EditContext;
            if (editContext is not null && !editContext.Validate())
            {
                var errors = editContext.GetValidationMessages().ToList();

                if (errors.Any())
                {
                    var errorMessage = string.Join("<br>", errors);
                    await myAlert.ShowErrorAlert("Error validation ", errorMessage, MyAlertType.warning);
                }

                return;
            }

            await Save();
        }
    }
}