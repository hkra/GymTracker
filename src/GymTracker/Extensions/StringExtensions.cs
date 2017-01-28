using System;
using System.Text;

namespace GymTracker.Extensions
{
    public static class StringExtensions
    {
        public static string FromBase64(this string encodedContent)
        {
            byte[] data = Convert.FromBase64String(encodedContent);
            return Encoding.UTF8.GetString(data);
        }
    }
}