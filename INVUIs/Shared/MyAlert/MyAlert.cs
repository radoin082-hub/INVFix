using BlazorBootstrap;
using Microsoft.JSInterop;
using Radzen;

namespace INVUIs.Shared.MyAlert;

public class MyAlert(IJSRuntime jsRuntime)
{
    public async Task ShowErrorAlert(string? title, string? message, MyAlertType myAlertType)
    {
        await jsRuntime.InvokeVoidAsync("Swal.fire", new
        {
            title,
            html = message,
            icon = myAlertType.ToString(),
            confirmButtonText = "OK"
        });
    }
}