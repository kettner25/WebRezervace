using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using WebRezervace.Models;
using WebRezervace.Data;

namespace WebRezervace.Controllers
{
    public class RezervaceController : Controller
    {
        private readonly WebRezervaceContext _context;
        public RezervaceController(WebRezervaceContext context){ _context = context; }

        public bool ZkontrolujSession()
        {
            bool Prihlaseny = false;

            if (HttpContext.Session.GetString("Uzivatel") != null && _context.Uzivatele.Where(u => u.Email == HttpContext.Session.GetString("Uzivatel")).FirstOrDefault() != null)
                Prihlaseny = true;

            return Prihlaseny;
        }

        [HttpGet]
        public IActionResult Formular()
        {
            if (!ZkontrolujSession())
            {
                HttpContext.Session.SetString("Chyba", "Pro vyplnění rezervace se přihlašte!");
                return Redirect("/Uzivatel/Prihlasit");
            }

            Rezervace rezervace = new Rezervace { Jmeno = "", Prijmeni = "", Email = "", Tel = 0, PocetOsob = 0, ZpravaProAdmina = "", Doba = 1 };
            
            if (ZkontrolujSession())
            {
                Uzivatel uzivatel = _context.Uzivatele.Where(u => u.Email == HttpContext.Session.GetString("Uzivatel")).First();

                rezervace.Jmeno = uzivatel.Jmeno != null ? uzivatel.Jmeno : "";
                rezervace.Prijmeni = uzivatel.Prijmeni != null ? uzivatel.Prijmeni : "";
                rezervace.Tel = uzivatel.Tel;
                rezervace.Email = uzivatel.Email;
            }

            ViewData["Chyba"] = HttpContext.Session.GetString("Chyba") == null ? "" : HttpContext.Session.GetString("Chyba");
            HttpContext.Session.SetString("Chyba", "");

            return View(rezervace);
        }
        [HttpPost]
        public IActionResult Formular(string jmeno, string prijmeni, int pocet_osob, DateTime datum, DateTime casOd, int doba, string email, int tel, string zprava)
        {
            if (jmeno == null || jmeno.Trim().Length == 0 || prijmeni == null || prijmeni.Trim().Length == 0 || pocet_osob == 0 || datum.Year == 1 || datum < DateTime.Now)
            {
                ViewData["Chyba"] = "Povinná pole nesmí být prázdná!";
                return View(new Rezervace { Jmeno = jmeno, Prijmeni = prijmeni, PocetOsob = pocet_osob, Email = email, Tel = tel, ZpravaProAdmina = zprava });
            }
            if (casOd.Hour < 9 || casOd.Hour > 15 || casOd.Hour + doba > 16)
            {
                ViewData["Chyba"] = "Pracovní doba je pouze od 9-16h!";
                return View(new Rezervace { Jmeno = jmeno, Prijmeni = prijmeni, PocetOsob = pocet_osob, Email = email, Tel = tel, ZpravaProAdmina = zprava });
            }

            if (email != null)
            {
                string[] kontrolaEmail = email.Split("@");
                if (kontrolaEmail.Length != 2 || kontrolaEmail[0].Split(".").Length != 2 || kontrolaEmail[1].Split(".").Length != 2)
                {
                    ViewData["Chyba"] = "Zadejte platnou E-mailovou adresu!";
                    return View(new Rezervace { Jmeno = jmeno, Prijmeni = prijmeni, PocetOsob = pocet_osob, Email = email, Tel = tel, ZpravaProAdmina = zprava });
                }
            }
            else email = "";

            if (tel.ToString().Length != 9 && tel != 0)
            {
                ViewData["Chyba"] = "Zadejte platné telefonní číslo!";
                return View(new Rezervace { Jmeno = jmeno, Prijmeni = prijmeni, PocetOsob = pocet_osob, Email = email, Tel = tel, ZpravaProAdmina = zprava });
            }

            zprava = zprava == null ? "" : zprava;

            Uzivatel uzivatel =  _context.Uzivatele.Where(u => u.Email == HttpContext.Session.GetString("Uzivatel")).First();

            datum += casOd.TimeOfDay;

            Rezervace rezervace = new Rezervace { Jmeno = jmeno, Prijmeni = prijmeni, PocetOsob = pocet_osob, Doba = doba * 60, Datum = datum, Email = email, Tel = tel, ZpravaProAdmina = zprava, Autor = uzivatel };

            _context.Rezervace.Add(rezervace);
            _context.SaveChanges();

            return Redirect("/Home/Index");
        }

        /*
         <table>
        <tr>
            @for (int i = 9; i <= 16; i++)
            {
                @:<th>@i - @(i+1)</th>
            }
        </tr>
        <tr>
            @for (int i = 9; i <= 16; i++)
            {
                int pocetmist = 0;
                @foreach (WebRezervace.Models.Rezervace rezervace in db.Rezervace.ToList())
                {
                    if (rezervace.Datum.Date == DateTime.Now && rezervace.Datum.TimeOfDay == TimeSpan.FromHours(i))
                    {
                        pocetmist += rezervace.PocetOsob;
                    }
                }
                @:<td>@pocetmist/25</td>
            }
        </tr>
    </table>
         */
    }
}
