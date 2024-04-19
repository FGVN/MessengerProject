using System.Text;

public class CreateChatCommandHandler
{
    private readonly HttpWrapper _httpWrapper;
    private readonly LocalStorageUtils _localStorageUtils;

    public CreateChatCommandHandler(HttpWrapper httpWrapper, LocalStorageUtils localStorageUtils)
    {
        _httpWrapper = httpWrapper;
        _localStorageUtils = localStorageUtils;
    }

    public async Task<Guid> Handle(string contactUsername)
    {
        try
        {
            // Get the JWT token from local storage
            var token = await _localStorageUtils.GetJwtTokenFromLocalStorage();

            // Send the request to create a chat
            var response = await _httpWrapper.PostAsync<object, string>(
                $"Chats/create?contactUsername={contactUsername}", null, token);

            // Extract the chat ID from the response
            var chatId = Guid.Parse(response);

            return chatId;
        }
        catch (Exception ex)
        {
            // Handle exception
            Console.WriteLine($"Error creating chat: {ex.Message}");
            throw;
        }
    }
}
