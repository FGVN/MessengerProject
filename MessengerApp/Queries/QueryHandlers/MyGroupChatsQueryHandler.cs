using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MessengerApp.Models;

public class MyGroupChatsQueryHandler
{
    private readonly HttpWrapper _httpWrapper;
    private readonly LocalStorageUtils _localStorageUtils;

    public MyGroupChatsQueryHandler(HttpWrapper httpWrapper, LocalStorageUtils localStorageUtils)
    {
        _httpWrapper = httpWrapper;
        _localStorageUtils = localStorageUtils;
    }

    public async Task<IEnumerable<GroupChat>> Handle()
    {
        try
        {
            var token = await _localStorageUtils.GetJwtTokenFromLocalStorage();

            var response = await _httpWrapper.GetAsync<IEnumerable<GroupChat>>(
                "https://localhost:7287/api/GroupChats/myGroups", token);

            return response;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting user's group chats: {ex.Message}");
            throw;
        }
    }
}
