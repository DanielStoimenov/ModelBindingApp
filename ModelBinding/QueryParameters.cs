namespace ModelBinding;

public class QueryParameters
{
    public string? OrderBy { get; set; }

    public bool? Descending { get; set; }

    public bool? IsConjunction { get; set; }

    public List<FilterParameters>? FilterParametersList { get; set; }
}