using System.IO.Pipelines;
using System.Net.Sockets;
using System.Threading.Tasks.Sources;

namespace Gruda.HttpServer.Transport.KestrelBased;

//From: https://github.com/dotnet/aspnetcore/blob/main/src/Servers/Kestrel/Transport.Sockets/src/Internal/SocketAwaitableEventArgs.cs#L10-L77
internal class SocketAwaitableEventArgs : SocketAsyncEventArgs, IValueTaskSource<SocketOperationResult>
{
    private static readonly Action<object?> _continuationCompleted = _ => { };

    private readonly PipeScheduler _ioScheduler;

    // There are places where we read the _continuation field and then read some other state which we assume to be consistent
    // with the value we read in _continuation. Without a fence, those secondary reads could be reordered with respect to the first.
    // https://github.com/dotnet/runtime/pull/84432
    // https://github.com/dotnet/aspnetcore/issues/50623
    private volatile Action<object?>? _continuation;

    public SocketAwaitableEventArgs(PipeScheduler ioScheduler)
        : base(unsafeSuppressExecutionContextFlow: true)
    {
        _ioScheduler = ioScheduler;
    }

    protected override void OnCompleted(SocketAsyncEventArgs _)
    {
        var c = _continuation;

        if (c != null || (c = Interlocked.CompareExchange(ref _continuation, _continuationCompleted, null)) != null)
        {
            var continuationState = UserToken;
            UserToken = null;
            _continuation = _continuationCompleted; // in case someone's polling IsCompleted

            _ioScheduler.Schedule(c, continuationState);
        }
    }

    public SocketOperationResult GetResult(short token)
    {
        _continuation = null;

        if (SocketError != SocketError.Success)
        {
            return new SocketOperationResult(CreateException(SocketError));
        }

        return new SocketOperationResult(BytesTransferred);
    }

    protected static SocketException CreateException(SocketError e)
    {
        return new SocketException((int)e);
    }

    public ValueTaskSourceStatus GetStatus(short token)
    {
        return !ReferenceEquals(_continuation, _continuationCompleted) ? ValueTaskSourceStatus.Pending :
                SocketError == SocketError.Success ? ValueTaskSourceStatus.Succeeded :
                ValueTaskSourceStatus.Faulted;
    }

    public void OnCompleted(Action<object?> continuation, object? state, short token, ValueTaskSourceOnCompletedFlags flags)
    {
        UserToken = state;
        var prevContinuation = Interlocked.CompareExchange(ref _continuation, continuation, null);
        if (ReferenceEquals(prevContinuation, _continuationCompleted))
        {
            UserToken = null;
            ThreadPool.UnsafeQueueUserWorkItem(continuation, state, preferLocal: true);
        }
    }
}