using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace ModelBinding.Controllers;

[ApiController]
[Route("[controller]")]
public partial class QueryParametersController : ControllerBase
{
    [HttpPost]
    public List<Person> Post([ModelBinder(BinderType = typeof(QueryParameterModelBinder))] QueryParameters queryParameters, List<Person> people)
    {
        people.Add(new Person(people.Count + 1, "Korava", "9459049285"));
        people.Add(new Person(people.Count + 1, "Slava", "9320574829"));
        people.Add(new Person(people.Count + 1, "Prava", "9413072388"));


        FilterParameters? filters = queryParameters.FilterParametersList.FirstOrDefault();

        List<Person> result = new();

        switch (filters!.Type) 
        {
            case "startswith":
                result = people.Where(p => p.Egn.StartsWith(filters.Value)).ToList();
                break;
            case "endswith":
                result = people.Where(p => p.Egn.EndsWith(filters.Value)).ToList();
                break;
            case "contains":
                result = people.Where(p => p.Egn.Contains(filters.Value)).ToList();
                break;
        }

        if (queryParameters.Descending == true)
        {
            var ordredResult = new List<Person>();

            ordredResult = result.OrderByDescending(x => x.Id).ToList();

            return ordredResult;
        }
        
        return result;
    }

    // TODO expressions, delegates, functions, lambdas

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
            Descending = bool.Parse(descendingValueProviderResult.FirstValue!),
            IsConjunction = bool.Parse(isConjunctionValueProviderResult.FirstValue!),
            FilterParametersList = filters!
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