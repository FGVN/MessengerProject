using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerDataAccess.Models.Messages
{
    public class SendMessageDTO
    {
        public Guid ChatId { get; set; }
        public string SenderId { get; set; }
        public string Message { get; set; }
    }
}
