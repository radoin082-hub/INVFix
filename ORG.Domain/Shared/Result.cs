using System.Diagnostics.CodeAnalysis;

namespace ORG.Domain.Shared;

public class Result
{
    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
        {
            throw new InvalidCastException();
        }

        if (!isSuccess && error == Error.None)
        {
            throw new InvalidCastException();
        }

        IsSuccess = isSuccess;
        Error = error;
    }
    protected Result(bool isSuccess, List<Error> error)
    {
        if (isSuccess && error.Any())
        {
            throw new InvalidCastException();
        }

        if (!isSuccess && error.Any()==false)
        {
            throw new InvalidCastException();
        }

        IsSuccess = isSuccess;
        Errors = error;
    }
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    public Error Error { get; }
    public List<Error> Errors { get; }

    public static Result Success() => new(true, Error.None);

    public static Result Failure(Error error) => new(false, error);
    public static Result Failure(List<Error> errors) => new(false, errors);

    public static Result<T> Success<T>(T value) => new(value, true, Error.None);

    public static Result<T> Failure<T>(Error error) => new(default!, false, error);

    public static implicit operator Result(Error error) =>
        Failure(error);

    public static Result<T> Create<T>(T? value) =>
        value is not null ? Success(value) : Failure<T>(Error.NullValue);
}

public class Result<T> : Result
{
    private readonly T? _value;

    protected internal Result(T value, bool isSuccess, Error error) : base(isSuccess, error)
    {
        _value = value;
    }

    [NotNull]
    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("The value of a failure result can not be accessed");

    public static implicit operator Result<T>(Error error) =>
        Failure<T>(error);

    public static implicit operator Result<T>(T? value) => Create(value);
}