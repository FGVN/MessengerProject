﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DataAccess.Models;

public class GroupChatDTO
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }
    [MaxLength]
    public string Description { get; set; }
    [Required]
    public DateTime CreatedAt { get; set; }
}