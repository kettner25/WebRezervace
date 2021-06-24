using System.ComponentModel.DataAnnotations;
using System;

namespace WebRezervace.Models
{
    public class Rezervace
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string Jmeno { get; set; }
        [Required]
        public string Prijmeni { get; set; }
        [Required]
        public int PocetOsob { get; set; }
        [Required]
        public DateTime Datum { get; set; }
        [Required]
        //Doba psaná v Minutách
        public int Doba { get; set; }
        public string Email { get; set; }
        public int Tel { get; set; }
        public string ZpravaProAdmina { get; set; }
        [Required]
        virtual public Uzivatel Autor { get; set; }
    }
}
