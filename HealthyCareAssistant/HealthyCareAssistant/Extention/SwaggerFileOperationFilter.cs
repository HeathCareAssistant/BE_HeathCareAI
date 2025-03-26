using HealthyCareAssistant.ModelViews.DrugModelViews;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HealthyCareAssistant.Extention
{
    public class SwaggerFileOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            foreach (var parameter in context.ApiDescription.ParameterDescriptions)
            {
                if (parameter.Type == typeof(DrugImageUploadModel)) 
                {
                    operation.RequestBody = new OpenApiRequestBody
                    {
                        Content = new Dictionary<string, OpenApiMediaType>
                        {
                            ["multipart/form-data"] = new OpenApiMediaType
                            {
                                Schema = new OpenApiSchema
                                {
                                    Type = "object",
                                    Properties = new Dictionary<string, OpenApiSchema>
                                    {
                                        { "file", new OpenApiSchema { Type = "string", Format = "binary" } }
                                    }
                                }
                            }
                        }
                    };
                }
            }
        }
    }
}
