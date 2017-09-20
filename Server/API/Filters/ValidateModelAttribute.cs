using QIP.API.MediaTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System.Linq;
using System.Net;
using System.Text;

namespace QIP.API.Filters {
    public class ValidateModelAttribute : ActionFilterAttribute { //TODO Rever o Problem+JSON
        private static readonly MediaTypeHeaderValue MEDIA_TYPE = new MediaTypeHeaderValue("application/problem+json");
        private static readonly string INDENTATION = "  ";

        public override void OnActionExecuting(ActionExecutingContext context) {
            if(!context.ModelState.IsValid) {
                var problem = GetModelValidationProblem(context.ModelState);

                context.Result = new BadRequestObjectResult(JsonConvert.SerializeObject(problem)) {
                    ContentTypes = { MEDIA_TYPE }
                };
            }
        }

        private static ProblemPlusJson GetModelValidationProblem(ModelStateDictionary modelState) {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("The following fields of the Model are invalid:");

            string details = modelState.Values
                .SelectMany(ms => ms.Errors)
                .Select(e => e.ErrorMessage)
                .Aggregate(
                    builder,
                    (b, e) => {
                        b.Append(INDENTATION);
                        b.AppendLine(e);
                        return b;
                    },
                    b => b.ToString()
                );

            return new ProblemPlusJson() {
                Status = HttpStatusCode.BadRequest,
                Title = "The Model is invalid",
                Details = details
            };
        }
    }
}
