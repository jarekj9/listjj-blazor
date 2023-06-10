namespace Listjj_frontend.Services
{
    public interface IApiClient
    {
        Task<(TResponse Result, HttpResponseMessage HttpResponse)> Get<TResponse>(string urlPart);
        Task<(TResponse Result, HttpResponseMessage HttpResponse)> Post<TRequest, TResponse>(string urlPart, TRequest requestData);
    }
}