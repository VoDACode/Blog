using Microsoft.Net.Http.Headers;

namespace Blog.Server.Tools
{
    public static class MultipartRequestHelper
    {
        public static bool IsMultipartContentType(string? contentType)
        {
            return !string.IsNullOrEmpty(contentType) && contentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static string GetBoundary(MediaTypeHeaderValue contentType)
        {
            var boundary = HeaderUtilities.RemoveQuotes(contentType.Boundary).Value;
            if (string.IsNullOrWhiteSpace(boundary))
                throw new BadHttpRequestException("Missing content-type boundary.");

            return boundary;
        }
    }
}
