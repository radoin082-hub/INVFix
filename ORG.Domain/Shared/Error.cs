namespace ORG.Domain.Shared;

public record Error
{
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);
    public static readonly Error NullValue = new(
        "General.Null",
        "Null value was provided",
        ErrorType.Failure);

    protected Error(string code, string description, ErrorType type)
    {
        Code = code;
        Description = description;
        Type = type;
    }

    protected Error(Exception ex)
    {
        Code = "Error.Exception";
        Description = ex.Message;
        Type = ErrorType.Exception;
    }
    public string Code { get; }

    public string Description { get; }

    public ErrorType Type { get; }

    public static Error Failure(string code, string description) =>
        new(code, description, ErrorType.Failure);

    public static Error NotFound(string code, string description) =>
        new(code, description, ErrorType.NotFound);

    public static Error Problem(string code, string description) =>
        new(code, description, ErrorType.Problem);
    public static Error Exception(Exception ex) =>
        new(ex);

    public static Error Conflict(string code, string description) =>
        new(code, description, ErrorType.Conflict);
}