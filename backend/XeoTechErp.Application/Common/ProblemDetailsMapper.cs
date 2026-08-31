using XeoTechErp.Application.Common;

namespace XeoTechErp.Application.Common;

public static class ProblemDetailsMapper
{
    public static (int StatusCode, object Body) Map(Error? error)
    {
        if (error is null) return (400, new { code = "REQUEST_FAILED", message = "The request could not be completed." });
        var status = error.Code.EndsWith("_NOT_FOUND", StringComparison.Ordinal) ? 404
            : error.Code.EndsWith("_EXISTS", StringComparison.Ordinal) || error.Code is "CUSTOMER_ON_HOLD" or "INSUFFICIENT_STOCK" ? 409
            : 400;
        return (status, new { code = error.Code, message = error.Message });
    }
}
