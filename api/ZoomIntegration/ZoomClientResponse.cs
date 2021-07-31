namespace Lingua.ZoomIntegration
{
    public class ZoomClientResponse<T> 
    {
        public ZoomClientResponse(T response, AccessTokens newTokens)
        {
            NewTokens = newTokens;
            Response = response;
        }

        public T Response { get; }
        public AccessTokens NewTokens { get; }
    }
}