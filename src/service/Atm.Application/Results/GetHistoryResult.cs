using Atm.Domain;

namespace Atm.Application.Results;

public record GetHistoryResult(GetHistoryStatus Status, IReadOnlyList<Operation>? Operations, long? NextCursor);
