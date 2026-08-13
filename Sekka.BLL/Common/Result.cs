namespace Sekka.BLL.Common
{
    public record Result(bool success, string? error = null, ResultKind kind = ResultKind.Ok)
    {
        public static Result OK() => new(true);
        public static Result Fail(string error, ResultKind kind = ResultKind.Conflict) => new(false, error, kind);
        public static Result NotFound(string error = "Not Found!") => new(false, error, ResultKind.NotFound);
        public static Result Validation(string error) => new(false, error, ResultKind.ValidationFailed);


    }
    public record Result<T>(bool success, T? Value, string? error = null, ResultKind kind = ResultKind.Ok)
    {
        public static Result<T> OK(T value) => new(true, value);
        public static Result<T> Fail(string error, ResultKind kind = ResultKind.Conflict) => new(false, default, error, kind);
        public static Result<T> NotFound(string error = "Not Found!") => new(false, default, error, ResultKind.NotFound);
        public static Result<T> Validation(string error) => new(false, default, error, ResultKind.ValidationFailed);

    }
}
