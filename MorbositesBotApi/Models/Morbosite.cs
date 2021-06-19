using System;

namespace MorbositesBotApi.Models
{
    public class Morbosite
    {
        public long Id { get; set; }

        public long UserId { get; set; }

        public string Username { get; set; }

        public DateTime JoinedOn { get; set; }

        public DateTime? LastMessageOn { get; set; }
    }
}
