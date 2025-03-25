using BlazorBootstrap;
using Microsoft.JSInterop;
using Radzen;

namespace INVUIs.Shared.MyAlert;

public class MyAlert(IJSRuntime jsRuntime)
{
    public async Task ShowAlert(string? title, string? message, MyAlertType myAlertType)
    {
        await jsRuntime.InvokeVoidAsync("Swal.fire", new
        {
            title,
            html = message,
            icon = myAlertType.ToString(),
            confirmButtonText = "OK"
        });
    }

    public async Task ShowToast(string? title, string? message, MyAlertType myAlertType)
    {
        await jsRuntime.InvokeVoidAsync("Swal.fire", new
        {
            title,
            html = message,
            toast = true,
            position = "top-end",
            showConfirmButton = false,
            timer = 3000,
            timerProgressBar = true,
            icon = myAlertType.ToString(),
        });
    }
}