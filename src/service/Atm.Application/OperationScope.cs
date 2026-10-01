using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Atm.Application;

public static class OperationScope
{
    public static IDisposable Begin(ILogger logger, string operationName, IReadOnlyDictionary<string, string?> tags)
    {
        Activity? activity = AtmTelemetry.ActivitySource.StartActivity(operationName);

        foreach (KeyValuePair<string, string?> tag in tags)
        {
            if (tag.Value == null)
                continue;

            activity?.SetTag(tag.Key, tag.Value);
            activity?.SetBaggage(tag.Key, tag.Value);
        }

        IDisposable? logScope = logger.BeginScope(tags);

        return new CompositeDisposable(activity, logScope);
    }

    private sealed class CompositeDisposable : IDisposable
    {
        private readonly IDisposable? _first;
        private readonly IDisposable? _second;

        public CompositeDisposable(IDisposable? first, IDisposable? second)
        {
            _first = first;
            _second = second;
        }

        public void Dispose()
        {
            _first?.Dispose();
            _second?.Dispose();
        }
    }
}
