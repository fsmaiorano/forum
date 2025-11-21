namespace Forum.Domain.Base;

public abstract class Pagination
{
    public int Page { get; set; }
    public int PageSize { get; set; }
}