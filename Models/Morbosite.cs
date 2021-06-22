using System;
using System.ComponentModel.DataAnnotations;

namespace MorbositesBotApi.Models
{
    public class Morbosite
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public long UserId { get; set; }

        [Required]
        public long ChatId { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public DateTime JoinedOn { get; set; }

        public DateTime? LastMessageOn { get; set; }
    }
}
