using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeefServer.Database
{
    [Table("user")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set;}

        [Required]
        [StringLength(32)]
        public string username { get; set; }

        [Required]
        [StringLength(32)]
        public string password { get; set; }

        [Required]
        [StringLength(32)]
        public string role { get; set; }

        [StringLength(64)]
        public string email { get; set; }

        [StringLength(64)]
        public string phone { get; set; }

        [MaxLength(4096)]
        public byte[]  image { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? register_time { get; set; }

        public User()
        {
            register_time = DateTime.Now;
        }
    }
}
