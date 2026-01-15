using AccountMaster.Grpc;
using Application.Services.Interfaces;
using Grpc.Core;
using Presentation.Grpc.Mappers;

namespace Presentation.Grpc.Services;

public class GrpcAccountService : AccountService.AccountServiceBase
{
    private readonly IAccountService _accountService;

    public GrpcAccountService(IAccountService accountService)
    {
        _accountService = accountService;
    }

    public override async Task<GetAccountResponse> GetAccount(GetAccountRequest request, ServerCallContext context)
    {
        Application.Models.Account result = await _accountService.GetAccountAsync(request.AccountId, default);
        GetAccountResponse response = GrpcMapper.ToGrpcResponse(result);
        return response;
    }

    public override async Task<CheckBanStatusResponse> CheckBanStatus(CheckBanStatusRequest request, ServerCallContext context)
    {
        bool result = await _accountService.CheckBanStatusAsync(request.AccountId, default);
        return new CheckBanStatusResponse { IsBanned = result };
    }
}