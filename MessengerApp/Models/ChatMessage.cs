using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class ChatMessage
{
    public int Id { get; set; }
    public Guid ChatId { get; set; }
    public string SenderId { get; set; }
    public string Message { get; set; }
    public DateTime TimeStamp { get; set; }
}

