using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace TaskManager.API.Filters;

public class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type.IsEnum)
        {
            schema.Enum.Clear();
            Enum.GetNames(context.Type)
                .ToList()
                .ForEach(name =>
                {
                    var member = context.Type.GetMember(name).First();
                    var description = member.GetCustomAttribute<System.ComponentModel.DescriptionAttribute>()?.Description;
                    schema.Enum.Add(new OpenApiString(name));
                    schema.Description = description ?? name;
                });
        }
    }
} 