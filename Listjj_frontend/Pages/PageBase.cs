using Microsoft.AspNetCore.Components;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Listjj.Infrastructure.ViewModels;
using Listjj_frontend.Services.Abstract;
using Microsoft.AspNetCore.Components.Server.Circuits;

namespace Listjj_frontend.Pages
{
    public class PageBase : ComponentBase
    {
        [Inject] public IItemService ItemService { get; set; }
        [Inject] public ICategoryService CategoryService { get; set; }
        [Inject] protected NavigationManager NavManager { get; set; }
        [Inject] protected Data.AppState appState { get; set; }
        [Inject] protected IFileService FileService { get; set; }
        [Inject] protected ITagsService TagsService { get; set; }
        [Inject] protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        [Inject] protected CircuitHandler CircuitHandler { get; set; }
        protected IEnumerable<Claim> _claims = Enumerable.Empty<Claim>();
        protected List<string> TagsSelection { get; set; } = new List<string>();
        protected List<ListItemViewModel> Items { get; set; } = new List<ListItemViewModel>();
        protected List<CategoryViewModel> Categories { get; set; }
        protected bool isDrawerOpen = true;
        protected bool isLoaded;


        protected async Task LoadData()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            if (user.Identity.IsAuthenticated)
            {
                _claims = user.Claims;
                string id = user.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                Guid.TryParse(id, out var idGuid);
                string username = user.FindFirst(c => c.Type == ClaimTypes.Name)?.Value;
                appState.SetLogin(true, idGuid, username);  // TODO: should be moved to some login class ?
            }
            Categories = await CategoryService.GetAllByUserId();

            appState.RecentCategoryId = await CategoryService.GetRecentCategoryByUserId();
            if (appState.RecentCategoryId != Guid.Empty)
            {
                Items = await ItemService.GetItemsByCategoryId(appState.RecentCategoryId);
            }
            else
            {
                Items = await ItemService.GetAllByUserId(appState.UserId);
            }
            TagsSelection = await TagsService.GetByUserId(appState.UserId);
            isLoaded = true;
        }

    }
}
