namespace Infra;

public class ListResult<T>
{
    public ListResult()
    {
        Data = new List<T>();
        RelatedItems = new ExpandoObject();
        Metadata = new ListResultMetadata();
    }

    public List<T> Data { get; set; }

    public ListResultMetadata Metadata { get; set; }


    public ListResult<TOut> CopyFrom<TOut, TIn>(ListResult<TIn> source, Func<TIn, TOut> projector)
    {
        var target = new ListResult<TOut>();
        target.Metadata = new ListResultMetadata();
        target.Metadata.PageNumber = source.Metadata.PageNumber;
        target.Metadata.PageSize = source.Metadata.PageSize;
        target.Metadata.TotalCount = source.Metadata.TotalCount;
        target.Data = new List<TOut>();
        foreach (var item in source.Data)
        {
            target.Data.Add(projector(item));
        }
        return target;
    }

    public decimal Milliseconds { get; set; }

    public dynamic RelatedItems { get; set; }
}
