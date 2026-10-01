using System.Text.Json.Serialization;

namespace Atm.Gateway.DTO;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(WithdrawOperationDto), "withdraw")]
[JsonDerivedType(typeof(DepositOperationDto), "deposit")]
public abstract record OperationDto(decimal Amount, DateTime Timestamp);
