using Listjj.Infrastructure.ViewModels;
using ListjjFrontEnd.Services.Abstract;

namespace ListjjFrontEnd.Services
{
    public class UserService : IUserService
    {
        private readonly IApiClient apiClient;
        public UserService(IApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        public async Task<List<UserViewModel>> GetAllUsers()
        {
            var response = await apiClient.Get<List<UserViewModel>>($"/api/users/all");
            var users = response.HttpResponse.IsSuccessStatusCode ? response.Result : new List<UserViewModel>();
            return users;
        }

        public async Task<bool> AddorUpdate(UserViewModel user)
        {
            var response = await apiClient.Post<UserViewModel,bool>($"/api/users/addorupdate", user);
            var result = response.HttpResponse.IsSuccessStatusCode ? response.Result : false;
            return result;
        }

        public async Task<bool> Delete(UserViewModel user)
        {
            var response = await apiClient.Post<UserViewModel, bool>($"/api/users/delete", user);
            var result = response.HttpResponse.IsSuccessStatusCode ? response.Result : false;
            return result;
        }
    }
}