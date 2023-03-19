namespace Infra;

public static class TypeExtensions
{
    public static bool IsEnumerable(this System.Type type)
    {
        return type.GetInterface("IEnumerable") != null;
    }

    public static DataTable ToTableDefinition(this System.Type type)
    {
        try
        {
            var table = new DataTable();
            var properties = type.GetProperties().Where(i => i.Name != "RelatedItems");
            foreach (var property in properties)
            {
                if (Nullable.GetUnderlyingType(property.PropertyType) != null)
                {
                    table.Columns.Add(property.Name, property.PropertyType.GetGenericArguments()[0]);
                }
                else
                {
                    table.Columns.Add(property.Name, property.PropertyType);
                }
            }
            return table;
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);
            throw new ServerException($"Type {type.FullName} can't be converted into DataTable");
        }
    }
}
