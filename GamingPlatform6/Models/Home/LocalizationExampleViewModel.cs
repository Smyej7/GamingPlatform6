using Microsoft.AspNetCore.Mvc.Localization;

//LocalizationExampleViewModel.cs===============================================
namespace GamingPlatform6.Models.Home
{
    public class LocalizationExampleViewModel
    {
        public string? IStringLocalizerInController { get; set; }
        public LocalizedHtmlString? IHtmlLocalizerInController { get; set; }
    }
}
