using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace ModelBinding.Controllers;

[ApiController]
[Route("[controller]")]
public class QueryParametersController : ControllerBase
{
    [HttpGet]
    public IList<Person> Get([ModelBinder(BinderType = typeof(QueryParameterModelBinder))] QueryParameters queryParameters, IList<Person> people)
    {
        people.Add(new Person(people.Count + 1, "Prava", "9402495038"));
        people.Add(new Person(people.Count + 1, "Slava", "9320574829"));
        people.Add(new Person(people.Count + 1, "Krava", "9413072388"));

        List<FilterParameters?> filters = queryParameters.FilterParametersList.ToList();

        foreach (var filter in filters)
        {
            switch (filter!.Type)
            {
                case "startswith":
                    people = people.Where(x => x.GetType().GetProperty(filter.Column)!.GetValue(x)!.ToString()!.StartsWith(filter.Value)).ToList();
                    break;
                case "endswith":
                    people = people.Where(x => x.GetType().GetProperty(filter.Column)!.GetValue(x)!.ToString()!.EndsWith(filter.Value)).ToList();
                    break;
                case "contains":
                    people = people.Where(x => x.GetType().GetProperty(filter.Column)!.GetValue(x)!.ToString()!.Contains(filter.Value)).ToList();
                    break;
            }
        }
        
        if (queryParameters.Descending == true)
        {
            people = people.OrderByDescending(x => x.GetType().GetProperty(queryParameters.OrderBy)).ToList()!;
        }

        return people;
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