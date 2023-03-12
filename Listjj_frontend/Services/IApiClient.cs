namespace Listjj_frontend.Services
{
    public interface IApiClient
    {
        Task<(TResponse Result, HttpResponseMessage HttpResponse)> Get<TResponse>(string url);
        Task<(TResponse Result, HttpResponseMessage HttpResponse)> Post<TRequest, TResponse>(string url, TRequest requestData);
    }
}