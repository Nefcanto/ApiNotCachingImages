namespace Infra;

public class Filter
{
    public string Property { get; set; }

    public FilterOperator Operator { get; set; }

    public string OperatorMathematicalNotation
    {
        get
        {
            return FilterOperatorNormalizer.NormalizeFilterOperator(Operator);
        }
    }

    public object Value { get; set; }

    public object[] Values { get; set; }
}
