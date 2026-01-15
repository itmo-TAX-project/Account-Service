using AccountMaster.Grpc;

namespace Presentation.Grpc.Mappers;

public static class GrpcMapper
{
    public static GetAccountResponse ToGrpcResponse(
        Application.Models.Account account)
    {
        if (account.Id == null) throw new NullReferenceException("account.Id is null which should not happen");

        var grpcAccount = new Account()
        {
            AccountId = account.Id.Value,
            Name = account.Name,
            Phone = account.Phone,
            Role = MapRole(account.Role),
        };

        return new GetAccountResponse()
        {
            Account = grpcAccount,
        };
    }

    private static Role MapRole(Application.Models.AccountRole role)
    {
        return role switch
        {
            Application.Models.AccountRole.Admin => Role.Admin,
            Application.Models.AccountRole.Driver => Role.Driver,
            Application.Models.AccountRole.Passenger => Role.Passenger,
            _ => Role.Unspecified,
        };
    }
}