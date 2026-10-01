namespace Atm.Gateway.DTO;

public record GetHistoryResponse(IReadOnlyList<OperationDto> Operations, string NextPageToken);
