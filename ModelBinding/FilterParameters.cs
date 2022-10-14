using System.ComponentModel.DataAnnotations;

namespace ModelBinding;

public class FilterParameters
{
    [MinLength(3)]
    [MaxLength(33)]
    public string? Column { get; set; }

    [MinLength(3)]
    [MaxLength(33)]
    public string? Value { get; set; }

    [MinLength(3)]
    [MaxLength(33)]
    public string? Type { get; set; }
}