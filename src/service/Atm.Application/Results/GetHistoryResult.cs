using Atm.Domain;

namespace Atm.Application.Results;

public record GetHistoryResult(GetHistoryStatus Status, IAsyncEnumerable<Operation>? History);
