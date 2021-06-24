using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using WebRezervace.Data;
using WebRezervace.Models;

namespace WebRezervace.Controllers
{
    public class UzivatelController : Controller
    {
        private readonly WebRezervaceContext _context;

        private readonly int AdminKod = 250;
        public UzivatelController(WebRezervaceContext context)
        {
            _context = context;
        }

        public bool ZkontrolujSession()
        {
            bool Prihlaseny = false;

            if (HttpContext.Session.GetString("Uzivatel") != null && _context.Uzivatele.Where(u => u.Email == HttpContext.Session.GetString("Uzivatel")).FirstOrDefault() != null)
                Prihlaseny = true;

            return Prihlaseny;
        }

        [HttpGet]
        public IActionResult Prihlasit()
        {
            ViewData["Chyba"] = HttpContext.Session.GetString("Chyba") == null ? "" : HttpContext.Session.GetString("Chyba");
            HttpContext.Session.SetString("Chyba", "");

            return View();
        }
        [HttpPost]
        public IActionResult Prihlasit(string email, string heslo)
        {
            if (email == null || email.Trim().Length == 0 || heslo == null || heslo.Trim().Length == 0)
            {
                HttpContext.Session.SetString("Chyba", "Email nebo heslo nesmí být prázdné!");
                return RedirectToAction("Prihlasit");
            }

            Uzivatel hledanyUzivatel = _context.Uzivatele.Where(u => u.Email == email).FirstOrDefault();

            if (hledanyUzivatel == null)
            {
                HttpContext.Session.SetString("Chyba", "Email nebo heslo je špatně!");
                return RedirectToAction("Prihlasit");
            }

            if (!BCrypt.Net.BCrypt.Verify(heslo, hledanyUzivatel.Heslo))
            {
                HttpContext.Session.SetString("Chyba", "Email nebo heslo je špatně!");
                return RedirectToAction("Prihlasit");
            }

            HttpContext.Session.SetString("Uzivatel", hledanyUzivatel.Email);

            return RedirectToAction("Profil");
        }
        [HttpGet]
        public IActionResult Zaregistrovat()
        {
            ViewData["Chyba"] = HttpContext.Session.GetString("Chyba") == null ? "" : HttpContext.Session.GetString("Chyba");
            HttpContext.Session.SetString("Chyba", "");

            return View();
        }
        [HttpPost]
        public IActionResult Zaregistrovat(string email, string heslo, string kontrolni_heslo, int admin_kod)
        {
            if (email == null || email.Trim().Length == 0 || heslo == null || heslo.Trim().Length == 0)
            {
                HttpContext.Session.SetString("Chyba", "Email nebo heslo nesmí být prázdné!");
                return RedirectToAction("Zaregistrovat");
            }
            string[] kontrolaEmail = email.Split("@");
            if (kontrolaEmail.Length != 2 || kontrolaEmail[0].Split(".").Length != 2 || kontrolaEmail[1].Split(".").Length != 2)
            {
                HttpContext.Session.SetString("Chyba", "Zadejte prosím platnou e-mailovou adresu!");
                return RedirectToAction("Zaregistrovat");
            }
            if (heslo != kontrolni_heslo)
            {
                HttpContext.Session.SetString("Chyba", "Hesla se neshodují!");
                return RedirectToAction("Zaregistrovat");
            }

            if (null != _context.Uzivatele.Where(u => u.Email == email).FirstOrDefault())
            {
                HttpContext.Session.SetString("Chyba", "Email nebo heslo je špatně!");
                return RedirectToAction("Zaregistrovat");
            }

            heslo = BCrypt.Net.BCrypt.HashPassword(heslo);

            Uzivatel uzivatel = new Uzivatel() { Email = email, Heslo = heslo, AdminOpravneni = false};

            _context.Uzivatele.Add(uzivatel);
            _context.SaveChanges();

            return RedirectToAction("Prihlasit");
        }

        public IActionResult Profil()
        {
            if (!ZkontrolujSession())
                RedirectToAction("Prihlasit");

            ViewData["Chyba"] = HttpContext.Session.GetString("Chyba") == null ? "" : HttpContext.Session.GetString("Chyba");
            HttpContext.Session.SetString("Chyba", "");

            Uzivatel uzivatel = _context.Uzivatele.Where(u => u.Email == HttpContext.Session.GetString("Uzivatel")).First();

            uzivatel.Jmeno = uzivatel.Jmeno == null ? "" : uzivatel.Jmeno;
            uzivatel.Prijmeni = uzivatel.Prijmeni == null ? "" : uzivatel.Prijmeni;
            uzivatel.Rezervace = uzivatel.Rezervace.Count > 0 ? uzivatel.Rezervace : new List<Rezervace>();

            return View(uzivatel);
        }
        public IActionResult ZmenaHesla(string stavajici_heslo, string heslo, string kontrolni_heslo)
        {
            if (!ZkontrolujSession())
                RedirectToAction("Prihlasit");

            if (!BCrypt.Net.BCrypt.Verify(stavajici_heslo,_context.Uzivatele.Where(u => u.Email == HttpContext.Session.GetString("Uzivatel")).First().Heslo))
            {
                HttpContext.Session.SetString("Chyba", "Staré heslo bylo zadáno špatně!");
                return RedirectToAction("Profil");
            }
            if (heslo != kontrolni_heslo)
            {
                HttpContext.Session.SetString("Chyba", "Nová hesla se neshodují!");
                return RedirectToAction("Profil");
            }

            _context.Uzivatele.Where(u => u.Email == HttpContext.Session.GetString("Uzivatel")).First().Heslo = BCrypt.Net.BCrypt.HashPassword(heslo);
            _context.SaveChanges();

            return RedirectToAction("Profil");
        }
        public IActionResult ZmenaUdaju(string jmeno, string prijmeni, int tel, string email)
        {
            if (!ZkontrolujSession())
                RedirectToAction("Prihlasit");

            if (email == null || email == "")
                email = _context.Uzivatele.Where(u => u.Email == HttpContext.Session.GetString("Uzivatel")).First().Email;

            string[] kontrolaEmail = email.Split("@");
            if (kontrolaEmail.Length != 2 || kontrolaEmail[0].Split(".").Length != 2 || kontrolaEmail[1].Split(".").Length != 2)
            {
                HttpContext.Session.SetString("Chyba", "Zadejte platnou E-mailovou adresu!");
                return RedirectToAction("Profil");
            }
            if (tel.ToString().Count() != 9)
                tel = 0;

            _context.Uzivatele.Where(u => u.Email == HttpContext.Session.GetString("Uzivatel")).First().Jmeno = jmeno != "" ? jmeno : null;
            _context.Uzivatele.Where(u => u.Email == HttpContext.Session.GetString("Uzivatel")).First().Prijmeni = prijmeni != "" ? prijmeni : null;
            _context.Uzivatele.Where(u => u.Email == HttpContext.Session.GetString("Uzivatel")).First().Tel = tel;
            _context.Uzivatele.Where(u => u.Email == HttpContext.Session.GetString("Uzivatel")).First().Email = email;

            _context.SaveChanges();

            return RedirectToAction("Profil");
        }
        public IActionResult Odhlasit()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Prihlasit");
        }
    }
}
