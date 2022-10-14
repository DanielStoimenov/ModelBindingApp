using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace ModelBinding.Controllers;

[ApiController]
[Route("[controller]")]
public partial class QueryParametersController : ControllerBase
{
    public List<FilterParameters> FilterParameters { get; set; } = new();
    public QueryParameters QueryParameters { get; set; } = new();


    [HttpGet]
    public QueryParameters Get([ModelBinder(BinderType = typeof(QueryParameterModelBinder))] QueryParameters queryParameters)
    {
        List<Person> people = new();

        people.Add(new Person(1, "Iwan", "9312345678"));
        people.Add(new Person(2, "Zaprqn", "9412345678"));
        people.Add(new Person(3, "Dragan", "9354325678"));
        people.Add(new Person(4, "Petkan", "94888845678"));

        FilterParameters first = new();
        first.Column = "EGN";
        first.Value = "93";
        first.Type = "startsWith";

        // queryParameters.FilterParametersList.Where(x => x.Value == people.).Select(p => p.Egn.StartsWith(x.Value)));

        return queryParameters;
    }
}

public class QueryParameterModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext is null)
        {
            throw new ArgumentNullException(nameof(bindingContext));
        }

        ValueProviderResult filtersValueProviderResult = bindingContext.ValueProvider.GetValue("filters");
        ValueProviderResult orderByValueProviderResult = bindingContext.ValueProvider.GetValue("orderBy");
        ValueProviderResult descendingValueProviderResult = bindingContext.ValueProvider.GetValue("descending");
        ValueProviderResult isConjunctionValueProviderResult = bindingContext.ValueProvider.GetValue("isConjunction");

        if (filtersValueProviderResult == ValueProviderResult.None)
        {
            bindingContext.Result = ModelBindingResult.Failed();
            return Task.CompletedTask;
        }

        string? filtersValueAsString = filtersValueProviderResult.FirstValue;

        if (filtersValueAsString is null)
        {
            bindingContext.Result = ModelBindingResult.Failed();
            return Task.CompletedTask;
        }

        List<FilterParameters>? filters = JsonConvert.DeserializeObject<List<FilterParameters>>(filtersValueAsString);

        QueryParameters queryParameters = new()
        {
            OrderBy = orderByValueProviderResult.FirstValue,
            Descending = bool.Parse(descendingValueProviderResult.FirstValue),
            IsConjunction = bool.Parse(isConjunctionValueProviderResult.FirstValue),
            FilterParametersList = filters
        };

        if (queryParameters is null)
        {
            bindingContext.Result = ModelBindingResult.Failed();
            return Task.CompletedTask;
        }

        bindingContext.Result = ModelBindingResult.Success(queryParameters);

        return Task.CompletedTask;
    }
}