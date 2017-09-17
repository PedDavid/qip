using API.Interfaces.ServicesExceptions;
using API.MediaTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;

namespace API.Filters {
    public class ServicesExceptionFilterAttribute : ExceptionFilterAttribute { //TODO Rever o Problem+JSON
        private static readonly Microsoft.Net.Http.Headers.MediaTypeHeaderValue MEDIA_TYPE;
        private static readonly Dictionary<Type, Func<Exception, IActionResult>> _exceptionHandlers;

        static ServicesExceptionFilterAttribute() {
            MEDIA_TYPE = new Microsoft.Net.Http.Headers.MediaTypeHeaderValue("application/problem+json");

            _exceptionHandlers = new Dictionary<Type, Func<Exception, IActionResult>> {
                { typeof(NotFoundException), NotFoundExceptionHandler },
                { typeof(InvalidFieldsException), InvalidFieldsExceptionHandler },
                { typeof(InvalidChangeException), InvalidChangeExceptionHandler }
            };
        }

        public override void OnException(ExceptionContext context) {
            if(_exceptionHandlers.TryGetValue(context.Exception.GetType(), out Func<Exception, IActionResult> exceptionHandler)) {
                context.Result = exceptionHandler(context.Exception);
            }
        }

        private static IActionResult NotFoundExceptionHandler(Exception exception) {
            NotFoundException notFound = exception as NotFoundException;

            var problem = new ProblemPlusJson() {
                Status = HttpStatusCode.BadRequest,
                Title = "Resource Not Found",//TODO REVER
                Details = notFound.Message
            };

            var result = new NotFoundObjectResult(JsonConvert.SerializeObject(problem));
            result.ContentTypes.Add(MEDIA_TYPE);
            return result;
        }

        private static IActionResult InvalidFieldsExceptionHandler(Exception exception) {
            InvalidFieldsException notFound = exception as InvalidFieldsException;

            var problem = new ProblemPlusJson() {
                Status = HttpStatusCode.BadRequest,
                Title = "The request body has invalid fields",
                Details = notFound.Message
            };

            var result = new BadRequestObjectResult(JsonConvert.SerializeObject(problem));
            result.ContentTypes.Add(MEDIA_TYPE);
            return result;
        }

        private static IActionResult InvalidChangeExceptionHandler(Exception exception) {
            InvalidChangeException invalidChange = exception as InvalidChangeException;

            var problem = new ProblemPlusJson() {
                Status = HttpStatusCode.BadRequest,
                Title = "The change required is not valid",
                Details = invalidChange.Message
            };

            var result = new BadRequestObjectResult(JsonConvert.SerializeObject(problem));
            result.ContentTypes.Add(MEDIA_TYPE);
            return result;
        }
    }
}
