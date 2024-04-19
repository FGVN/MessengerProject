using System;
using System.Text.Json.Serialization;

public class ChatMenuItem
{
    public Guid ChatId { get; set; }
    public string Username { get; set; } // Username of the user
    public string ContactUsername { get; set; } // Username of the contact user
}
