using System.Net.Http.Json;
using SmartBank.Web.Models;

namespace SmartBank.Web.Services
{
    public interface ITransactionApiClient
    {
        Task<ApiResult> DepositAsync(DepositDto dto);
        Task<ApiResult> WithdrawAsync(WithdrawDto dto);
        Task<ApiResult> TransferAsync(TransferDto dto);
        Task<ApiResult<List<TransactionDto>>> GetStatementAsync(int accountId);
    }

    /// <summary>
    /// Talks to TransactionService through the Gateway route "/transaction/**" which forwards to
    /// TransactionController's "api/Transaction/**" routes.
    /// </summary>
    public class TransactionApiClient : ITransactionApiClient
    {
        private readonly HttpClient _http;

        public TransactionApiClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<ApiResult> DepositAsync(DepositDto dto)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("transaction/api/Transaction/deposit", dto);

                if (!response.IsSuccessStatusCode)
                {
                    return ApiResult.Fail(await response.ReadErrorMessageAsync());
                }

                return ApiResult.Ok();
            }
            catch (HttpRequestException ex)
            {
                return ApiResult.Fail($"Could not reach the Transaction service through the Gateway: {ex.Message}");
            }
        }

        public async Task<ApiResult> WithdrawAsync(WithdrawDto dto)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("transaction/api/Transaction/withdraw", dto);

                if (!response.IsSuccessStatusCode)
                {
                    return ApiResult.Fail(await response.ReadErrorMessageAsync());
                }

                return ApiResult.Ok();
            }
            catch (HttpRequestException ex)
            {
                return ApiResult.Fail($"Could not reach the Transaction service through the Gateway: {ex.Message}");
            }
        }

        public async Task<ApiResult> TransferAsync(TransferDto dto)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("transaction/api/Transaction/transfer", dto);

                if (!response.IsSuccessStatusCode)
                {
                    return ApiResult.Fail(await response.ReadErrorMessageAsync());
                }

                return ApiResult.Ok();
            }
            catch (HttpRequestException ex)
            {
                return ApiResult.Fail($"Could not reach the Transaction service through the Gateway: {ex.Message}");
            }
        }

        public async Task<ApiResult<List<TransactionDto>>> GetStatementAsync(int accountId)
        {
            try
            {
                var response = await _http.GetAsync($"transaction/api/Transaction/statement/{accountId}");

                if (!response.IsSuccessStatusCode)
                {
                    return ApiResult<List<TransactionDto>>.Fail(await response.ReadErrorMessageAsync());
                }

                var data = await response.Content.ReadFromJsonAsync<List<TransactionDto>>();
                return ApiResult<List<TransactionDto>>.Ok(data ?? new List<TransactionDto>());
            }
            catch (HttpRequestException ex)
            {
                return ApiResult<List<TransactionDto>>.Fail($"Could not reach the Transaction service through the Gateway: {ex.Message}");
            }
        }
    }
}
