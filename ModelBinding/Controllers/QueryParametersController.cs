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


    [HttpPost]
    public QueryParameters Post([ModelBinder(BinderType = typeof(QueryParameterModelBinder))] IList<FilterParameters> parameters, string? orderBy, bool? descending, bool? isConjunction)
    {
        for (int i = 0; i < parameters.Count; i++)
        {
            var filters = new FilterParameters
            {
                Column = parameters[i].Column,
                Value = parameters[i].Value,
                Type = parameters[i].Type
            };

            FilterParameters.Add(filters);
        }

        QueryParameters.FilterParametersList = FilterParameters;
        QueryParameters.Descending = descending;
        QueryParameters.OrderBy = orderBy;
        QueryParameters.IsConjunction = isConjunction;

        if (orderBy is not null)
        {
            switch (QueryParameters.OrderBy)
            {
                case "column":
                    _ = QueryParameters.FilterParametersList.OrderBy(x => x.Column);
                    break;
                case "value":
                    _ = QueryParameters.FilterParametersList.OrderBy(x => x.Value);
                    break;
                case "type":
                    _ = QueryParameters.FilterParametersList.OrderBy(x => x.Type);
                    break;
            }
        }
        
        return QueryParameters;
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
        // Check the value sent in
        ValueProviderResult valueProviderResult = bindingContext.ValueProvider.GetValue("filters");

        if (valueProviderResult == ValueProviderResult.None)
        {
            bindingContext.Result = ModelBindingResult.Failed();
            return Task.CompletedTask;
        }

        // Attempt to convert the input value
        string? valueAsString = valueProviderResult.FirstValue;
        if (valueAsString is null)
        {
            bindingContext.Result = ModelBindingResult.Failed();
            return Task.CompletedTask;
        }

        IList<FilterParameters>? filters = JsonConvert.DeserializeObject<IList<FilterParameters>>(valueAsString);
        if (filters is null)
        {
            bindingContext.Result = ModelBindingResult.Failed();
            return Task.CompletedTask;
        }

        bindingContext.Result = ModelBindingResult.Success(filters);
        return Task.CompletedTask;
    }
}
