using Cledev.Core.Events;
using OneOf;

namespace Cledev.Core.Results;

public sealed class Result : OneOfBase<Success, Failure>
{
    private Result(OneOf<Success, Failure> input) : base(input) { }

    public static implicit operator Result(Success success) => new(success);
    public static implicit operator Result(Failure failure) => new(failure);
    public static implicit operator Result(IEvent[] events) => new(new Success(events));
    
    public bool IsSuccess => IsT0;
    public bool IsNotSuccess => IsT1;
    public bool IsFailure => IsT1;

    public Success Success => AsT0;
    public Failure Failure => AsT1;

    public IEnumerable<IEvent> Events => AsT0.Events;
    
    public static Result Ok(Success success) => new(success);
    public static Result Ok(params IEvent[] events) => new(new Success(events));
    public static Result Fail(string errorCode = ErrorCodes.Error, string? title = null, string? description = null) => new(new Failure(errorCode, title, description));

    public bool TryPickSuccess(out Success success, out Failure failure) => TryPickT0(out success, out failure);
    public bool TryPickFailure(out Failure failure, out Success success) => TryPickT1(out failure, out success);
}

public sealed class Result<TResult> : OneOfBase<Success<TResult>, Failure>
{
    private Result(OneOf<Success<TResult>, Failure> input) : base(input) { }

    public static implicit operator Result<TResult>(Success<TResult> success) => new(success);
    public static implicit operator Result<TResult>(Failure failure) => new(failure);
    public static implicit operator Result<TResult>(TResult result) => new(new Success<TResult>(result));
    public static implicit operator Result<TResult>(IEvent[] events) => new(new Success<TResult>(events));
    
    public bool IsSuccess => IsT0;
    public bool IsNotSuccess => IsT1;
    public bool IsFailure => IsT1;

    public Success<TResult> Success => AsT0;
    public Failure Failure => AsT1;
    
    public new TResult? Value => AsT0.Result;
    public IEnumerable<IEvent> Events => AsT0.Events;

    public static Result<TResult> Ok(Success<TResult> success) => new(success);
    public static Result<TResult> Ok(TResult result, params IEvent[] events) => new(new Success<TResult>(result, events));
    public static Result<TResult> Fail(string errorCode = ErrorCodes.Error, string? title = null, string? description = null) => new(new Failure(errorCode, title, description));

    public bool TryPickSuccess(out Success<TResult> success, out Failure failure) => TryPickT0(out success, out failure);
    public bool TryPickFailure(out Failure failure, out Success<TResult> success) => TryPickT1(out failure, out success);
}
