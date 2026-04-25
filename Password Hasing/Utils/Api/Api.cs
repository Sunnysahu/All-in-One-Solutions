using System.Net;

namespace Password_Hasing.Utils.ApiError
{
    public class Api<T>
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
