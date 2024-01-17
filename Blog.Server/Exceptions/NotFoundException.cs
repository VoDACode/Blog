namespace Blog.Server.Exceptions
{
    public class NotFoundException : HttpRequestException
    {
        public NotFoundException(string message) : base(message)
        {
        }
        public NotFoundException() : base("Not Found")
        {
        }
    }
}
