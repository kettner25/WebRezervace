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
            /*if (!ZkontrolujSession())
            {
                HttpContext.Session.SetString("Chyba", "Pro vyplnění rezervace se přihlašte!");
                return Redirect("/Uzivatel/Prihlasit");
            }*/

            Rezervace rezervace = new Rezervace { Jmeno = "", Prijmeni = "", Email = "", Tel = 0, PocetOsob = 1, ZpravaProAdmina = "", Doba = 1, Datum = DateTime.Now.Date + TimeSpan.FromHours(9) };
            
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

            ViewBag.Data = _context.Rezervace.ToList();

            return View(rezervace);
        }
        [HttpPost]
        public IActionResult Formular(string jmeno, string prijmeni, int pocet_osob, DateTime datum, DateTime casOd, int doba, string email, int tel, string zprava)
        {
            if (jmeno == null || jmeno.Trim().Length == 0 || prijmeni == null || prijmeni.Trim().Length == 0 || pocet_osob == 0 || datum.Year == 1)
            {
                ViewData["Chyba"] = "Povinná pole nesmí být prázdná!";
                ViewBag.Data = _context.Rezervace.ToList();
                return View(new Rezervace { Jmeno = jmeno, Prijmeni = prijmeni, PocetOsob = pocet_osob, Email = email, Tel = tel, ZpravaProAdmina = zprava, Doba = doba, Datum = datum + casOd.TimeOfDay });
            }
            if (casOd.Hour < 9 || casOd.Hour > 15 || casOd.Hour + doba > 16 || datum < DateTime.Now)
            {
                ViewData["Chyba"] = "Pracovní doba je pouze od 9-16h! Rezervace přijímáme pouze na druhý den!";
                ViewBag.Data = _context.Rezervace.ToList();
                return View(new Rezervace { Jmeno = jmeno, Prijmeni = prijmeni, PocetOsob = pocet_osob, Email = email, Tel = tel, ZpravaProAdmina = zprava, Doba = doba, Datum = datum + casOd.TimeOfDay });
            }

            if (email != null)
            {
                string[] kontrolaEmail = email.Split("@");
                if (kontrolaEmail.Length != 2 || kontrolaEmail[0].Split(".").Length != 2 || kontrolaEmail[1].Split(".").Length != 2)
                {
                    ViewData["Chyba"] = "Zadejte platnou E-mailovou adresu!";
                    ViewBag.Data = _context.Rezervace.ToList();
                    return View(new Rezervace { Jmeno = jmeno, Prijmeni = prijmeni, PocetOsob = pocet_osob, Email = email, Tel = tel, ZpravaProAdmina = zprava, Doba = doba, Datum = datum + casOd.TimeOfDay });
                }
            }
            else email = "";

            if (tel.ToString().Length != 9 && tel != 0)
            {
                ViewData["Chyba"] = "Zadejte platné telefonní číslo!";
                ViewBag.Data = _context.Rezervace.ToList();
                return View(new Rezervace { Jmeno = jmeno, Prijmeni = prijmeni, PocetOsob = pocet_osob, Email = email, Tel = tel, ZpravaProAdmina = zprava, Doba = doba, Datum = datum + casOd.TimeOfDay });
            }

            zprava = zprava == null ? "" : zprava;

            Uzivatel uzivatel = new Uzivatel();

            if (ZkontrolujSession())
                uzivatel =  _context.Uzivatele.Where(u => u.Email == HttpContext.Session.GetString("Uzivatel")).First();

            datum += casOd.TimeOfDay;

            for (int i = datum.Hour; i < (datum + TimeSpan.FromHours(doba)).Hour; i++)
            {
                int CelkovyPocetOsob = 0;
                foreach (Rezervace rez in _context.Rezervace.ToList())
                {
                    if (rez.Datum.Date == datum.Date && rez.Datum.Hour <= i && rez.Datum.Hour + rez.Doba > i)
                    {
                        CelkovyPocetOsob += rez.PocetOsob;
                    }
                }
                if (CelkovyPocetOsob + pocet_osob > 20)
                {
                    ViewData["Chyba"] = $"Na {i}. hodinu již není možná rezervace! Limit osob naplněn!";
                    ViewBag.Data = _context.Rezervace.ToList();
                    return View(new Rezervace { Jmeno = jmeno, Prijmeni = prijmeni, PocetOsob = pocet_osob, Email = email, Tel = tel, ZpravaProAdmina = zprava, Doba = doba, Datum = datum + casOd.TimeOfDay });
                }
            }

            Rezervace rezervace = new Rezervace();

            if (ZkontrolujSession())
                rezervace = new Rezervace { Jmeno = jmeno, Prijmeni = prijmeni, PocetOsob = pocet_osob, Doba = doba * 60, Datum = datum, Email = email, Tel = tel, ZpravaProAdmina = zprava, Autor = uzivatel };
            else // Předpokládá se, že Adminský účet bude na stránce alespoň jeden! Admin účet slouží pouze pro kontrolu a správu rezervací.
                rezervace = new Rezervace { Jmeno = jmeno, Prijmeni = prijmeni, PocetOsob = pocet_osob, Doba = doba * 60, Datum = datum, Email = email, Tel = tel, ZpravaProAdmina = zprava, Autor = _context.Uzivatele.Where(u => u.AdminOpravneni == true).First() };

            _context.Rezervace.Add(rezervace);
            _context.SaveChanges();

            return Redirect("/Home/Index");
        }
    }
}
