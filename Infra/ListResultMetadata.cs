namespace Infra;

public class ListResultMetadata
{
    public int? PageNumber { get; set; }

    public int? PageSize { get; set; }

    public long? From
    {
        get
        {
            if (!HasData)
            {
                return null;
            }
            long? from = null;
            if (PageNumber.HasValue && PageSize.HasValue)
            {
                from = (PageNumber - 1) * PageSize + 1;
            }
            if (TotalCount == null)
            {
                return from;
            }
            if (from <= TotalCount)
            {
                return from;
            }
            return null;
        }
    }

    public long? To
    {
        get
        {
            if (!HasData)
            {
                return null;
            }
            long? to = null;
            if (From.HasValue)
            {
                to = From + (long)PageSize - 1;
            }
            if (TotalCount == null)
            {
                to = From + (long)DataCount - 1;
                return to;
            }
            if (to <= TotalCount)
            {
                return to;
            }
            return TotalCount;
        }
    }

    public long? TotalCount { get; set; }

    public long? PagesCount
    {
        get
        {
            if (!HasData)
            {
                return 0;
            }
            if (TotalCount == null)
            {
                return null;
            }
            var pagesCount = (int)Math.Ceiling((decimal)TotalCount.Value / (decimal)PageSize);
            return pagesCount;
        }
    }

    public bool HasData { get; set; }

    public int DataCount { get; set; }

    public bool? HasMore { get; set; }

    public long TimeInMilliseconds { get; set; }
}
