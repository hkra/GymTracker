using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace GymTracker.Extensions
{
    public static class HttpExtensions
    {
        public static HttpRequestMessage Authorize(this HttpRequestMessage request, string token)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return request;
        }
    }
}