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
    public string Get([ModelBinder(BinderType = typeof(QueryParameterModelBinder))] QueryParameters queryParameters)
    {
        List<Person> people = new();

        people.Add(new Person(1, "Iwan", "9312345678"));
        people.Add(new Person(2, "Zaprqn", "9412345678"));
        people.Add(new Person(3, "Dragan", "9354325678"));
        people.Add(new Person(4, "Petkan", "94888845678"));
        
        List<FilterParameters> first = queryParameters.FilterParametersList;
       
        people.Select(p => p.).Where(p => p.StartsWith(first.FirstOrDefault().Value));

        foreach (var person in people)
        {
            return $"Name: {person.Name}";
        }

        return "";
    }

    // TODO expressions, delegates, functions, lambdas

    private static IQueryable<Person> FilterTasks(QueryParameters parameters, IQueryable<Person> tasks)
    {

        List<FilterParameters> filters = parameters.FilterParametersList;

        /*if (parameters.FilterParametersList.Type is not null)
        {
            tasks = tasks.Where(x => x.Title.Contains(parameters.Title));
        }

        if (parameters.TypeId is not null)
        {
            tasks = tasks.Where(x => x.TypeId == parameters.TypeId);
        }

        if (parameters.StatusId is not null)
        {
            tasks = tasks.Where(x => x.StatusId == parameters.StatusId);
        }

        if (parameters.PriorityId is not null)
        {
            tasks = tasks.Where(x => x.PriorityId == parameters.PriorityId);
        }

        if (parameters.ImpactId is not null)
        {
            tasks = tasks.Where(x => x.ImpactId == parameters.ImpactId);
        }*/
        return tasks;
    }

    /*tasks = FilterTasks(parameters, tasks);

    tasks = parameters.OrderBy?.ToLower() switch
        {
            QueryParameters.OrderByTitle => tasks.OrderBy(x => x.Title),
            QueryParameters.OrderByType => tasks.OrderBy(x => x.Type),
            QueryParameters.OrderByStatus => tasks.OrderBy(x => x.Status),
            QueryParameters.OrderByPriority => tasks.OrderBy(x => x.Priority),
            QueryParameters.OrderByImpact => tasks.OrderBy(x => x.Impact),
            _ => tasks.OrderBy(x => x.Id)
        };*/

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