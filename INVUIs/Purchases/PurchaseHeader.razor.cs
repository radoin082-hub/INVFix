using System.ComponentModel.DataAnnotations;
using INV.App.Budgets;
using INV.Domain.Entities.Budget;
using INV.Domain.Entities.Purchases;
using INVUIs.Purchases.PurchaseModels;
using INVUIs.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace INVUIs.Purchases;

public partial class PurchaseHeader : ComponentBase
{
    [CascadingParameter] public PurchaseModel purchaseModel { get; set; } = new();
    [Parameter] public EventCallback OnCreate { get; set; }
    [Parameter] public EventCallback<PurchaseModel> OnPurchaseOrder { get; set; }
    [Inject] private IBudgetService budgetService { get; set; }
    [Inject] private IJSRuntime jsRuntime { set; get; }
    private MyAlert myAlert { set; get; }

    private int _selectedChapterCode;
    private int _selelctedArticleCode;
    private List<Article> articles = new();
    private List<Chapter> chapters = new();
    private Chapter chapter = new();
    private Article article = new();
    private int selectedChapterCode;
    private EditForm form;
    private bool displayVisa = false;
    private bool displayReject = false;

    public int SelectedArticleCode
    {
        get => _selelctedArticleCode;
        set
        {
            if (_selelctedArticleCode != value)
            {
                _selelctedArticleCode = value;
                purchaseModel.selectedArticle = value.ToString();
                LoadArticleTitle();
            }
        }
    }

    public int SelectedChapterCode
    {
        get => _selectedChapterCode;
        set
        {
            if (_selectedChapterCode != value)
            {
                _selectedChapterCode = value;
                purchaseModel.selectedChapter = value.ToString();
                LoadChapterTitle();
                LoadArticlesBycodeChapter();
            }
        }
    }

    private async Task create()
    {
        await OnCreate.InvokeAsync();
    }

    protected override void OnInitialized()
    {
        myAlert = new MyAlert(jsRuntime);
        base.OnInitialized();
    }

    protected override async Task OnInitializedAsync()
    {
        var result = await budgetService.GetAllChapitres();
        if (result.IsSuccess)
        {
            chapters = result.Value;
        }
    }

    private async void LoadChapterTitle()
    {
        var result = await budgetService.GetChapterByCode(SelectedChapterCode);

        if (result.IsSuccess)
        {
            chapter = result.Value;
        }

        purchaseModel.title_chapter = chapter.Name;
        StateHasChanged();
    }

    private async void LoadArticlesBycodeChapter()
    {
        var result = await budgetService.GetArticlesByCodeChapter(SelectedChapterCode);
        if (result.IsSuccess)
        {
            articles = result.Value;
        }

        StateHasChanged();
    }

    private async void LoadArticleTitle()
    {
        var result = await budgetService.GetArticlesByCodeArticle(SelectedArticleCode);
        if (result.IsSuccess)
        {
            article = result.Value;
        }

        purchaseModel.description_article = article.Name;
        StateHasChanged();
    }

    public async Task Save()
    {
        await OnPurchaseOrder.InvokeAsync(purchaseModel);
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
                    await myAlert.ShowErrorAlert("Error validation ", errorMessage);
                }

                return;
            }

            await Save();
        }
    }
}