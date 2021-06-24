using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace WebRezervace.Models
{
    public class Uzivatel
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Heslo { get; set; }
        public string Jmeno { get; set; }
        public string Prijmeni { get; set; }
        public int Tel { get; set; }
        [Required]
        public bool AdminOpravneni { get; set; }
        [Required]
        virtual public List<Rezervace> Rezervace { get; set; }
    }
}
