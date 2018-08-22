using Microsoft.AspNetCore.Mvc.Filters;
using Sikiro.SMS.Toolkits;

namespace Sikiro.SMS.Api
{
    public class GolbalExceptionAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (!context.ExceptionHandled)
            {
                context.Exception.WriteToFile();
            }

            base.OnException(context);
        }
    }
}
