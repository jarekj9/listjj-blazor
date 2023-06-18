using Listjj.Infrastructure.ViewModels;
using ListjjFrontEnd.Services.Abstract;

namespace ListjjFrontEnd.Services
{
    public class RolesService : IRolesService
    {
        private readonly IApiClient apiClient;
        public RolesService(IApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        public async Task<List<RoleViewModel>> GetAllRoles()
        {
            var response = await apiClient.Get<List<RoleViewModel>>($"/api/roles/all");
            var roles = response.HttpResponse.IsSuccessStatusCode ? response.Result : new List<RoleViewModel>();
            return roles;
        }
    }
}