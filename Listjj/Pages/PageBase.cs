using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Listjj.Abstract;
using Listjj.ViewModels;
using Listjj.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using System.Collections.Generic;
using System.Linq;
using List.Helpers;



namespace Listjj.Pages
{
    public class PageBase : ComponentBase
    {
        [Inject] protected NavigationManager NavManager { get; set; }
        [Inject] protected Data.AppState appState { get; set; }
        [Inject] protected IListItemService ListItemService { get; set; }
        [Inject] protected ICategoryService CategoryService { get; set; }
        [Inject] protected IFileService FileService { get; set; }
        [Inject] protected ITagsCacheService TagsCacheService { get; set; }
        [Inject] protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        protected IEnumerable<Claim> _claims = Enumerable.Empty<Claim>();

        protected List<ListItem> Items { get; set; }
        [Parameter] public List<ListItemViewModel> ItemsVm { get; set; }
        protected List<Category> Categories { get; set; }
        [Parameter] public List<CategoryViewModel> CategoriesVm { get; set; }
        protected bool isDrawerOpen = true;


        protected async Task LoadData()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            if (user.Identity.IsAuthenticated)
            {
                _claims = user.Claims;
                string id = user.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                string username = user.FindFirst(c => c.Type == ClaimTypes.Name)?.Value;
                appState.SetLogin(true, id, username);  // should be moved to some login class
            }
            Categories = await CategoryService.GetCategoriesByUserId(appState.UserId);
            CategoriesVm = MapperHelper.MapItems<Category, CategoryViewModel>(Categories);

            Items = await ListItemService.GetItemsByUserId(appState.UserId);
            ItemsVm = MapperHelper.MapItems<ListItem, ListItemViewModel>(Items);

        }

    }
}
