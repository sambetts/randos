using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Models;
using System.Threading.Tasks;

namespace Web
{
    public abstract class JavaScriptEnabledComponentBase : ComponentBase
    {
        [Inject] protected IJSRuntime JSRuntime { get; set; }

        #region JavaScript Calls

        protected async Task ReloadPage()
        {
            await JSRuntime.InvokeAsync<string>("reloadPage");
        }

        public async Task ShowJSAlert(string msg)
        {
            await JSRuntime.InvokeAsync<string>("showAlert", msg);
        }

        protected async Task<bool> ShowJSConfirm(string msg)
        {
            return await JSRuntime.InvokeAsync<bool>("confirm", msg);
        }

        protected async Task ShowJSError(NoReturnDataAPIResponse response)
        {
            await ShowJSAlert($"Error calling API: {response.ToSummary()}");
        }

        #endregion
    }
}
