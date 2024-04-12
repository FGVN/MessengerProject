public class EditMessageCommandHandler
{
    private readonly HttpWrapper _httpWrapper;
    private readonly LocalStorageUtils _localStorageUtils;

    public EditMessageCommandHandler(HttpWrapper httpWrapper, LocalStorageUtils localStorageUtils)
    {
        _httpWrapper = httpWrapper;
        _localStorageUtils = localStorageUtils;
    }

    public async Task Handle(EditMessageCommand command, string newMessage)
    {
        try
        {
            var token = await _localStorageUtils.GetJwtTokenFromLocalStorage();
            var url = $"https://localhost:7287/api/Messages/edit/";

            var updatedMessageDto = new EditMessageCommand
            {
                Id = command.Id,
                Message = newMessage
            };

            // Perform the HTTP PUT request to edit the message
            await _httpWrapper.PutAsync<EditMessageCommand, string>(url, updatedMessageDto, token);
        }
        catch (Exception ex)
        {
            // Handle exception
            Console.WriteLine($"Error editing message: {ex.Message}");
        }
    }
}
