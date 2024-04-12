public class DeleteMessageCommandHandler
{
    private readonly HttpWrapper _httpWrapper;
    private readonly LocalStorageUtils _localStorageUtils;

    public DeleteMessageCommandHandler(HttpWrapper httpWrapper, LocalStorageUtils localStorageUtils)
    {
        _httpWrapper = httpWrapper;
        _localStorageUtils = localStorageUtils;
    }

    public async Task Handle(int messageId)
    {
        try
        {
            var token = await _localStorageUtils.GetJwtTokenFromLocalStorage();
            var url = $"https://localhost:7287/api/Messages/delete/{messageId}";

            // Perform the HTTP DELETE request to delete the message
            await _httpWrapper.DeleteAsync(url, token);
        }
        catch (Exception ex)
        {
            // Handle exception
            Console.WriteLine($"Error deleting message: {ex.Message}");
        }
    }
}
