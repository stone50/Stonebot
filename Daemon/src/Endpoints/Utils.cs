namespace StonebotDaemon.Endpoints {
    using Microsoft.AspNetCore.Http;

    internal static class Utils {
        internal static IResult GetOkResult(string message) => Results.Ok(new { message });

        internal static IResult GetOkResult<T>(T data) => Results.Ok(new { data });

        internal static IResult GetProblemResult(string detail, int statusCode) => Results.Problem(detail: detail, statusCode: statusCode);

        internal static IResult GetConfigurationRequiredResult(string configValueName) => GetProblemResult(
            $"The config value `{configValueName}` is empty.",
            StatusCodes.Status428PreconditionRequired
        );
    }
}