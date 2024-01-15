namespace Blog.Server.Enums
{
    public enum AuthorizeType
    {
        Any = 0x2,
        Cookie = 0x4,
        JWT = 0x8,
        User = 12 // 0x4 | 0x8
    }
}
