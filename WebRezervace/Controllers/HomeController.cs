﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using WebRezervace.Data;
using WebRezervace.Models;

namespace WebRezervace.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly WebRezervaceContext _context;

        public HomeController(ILogger<HomeController> logger, WebRezervaceContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            List<Rezervace> rezervace = _context.Rezervace.ToList();

            rezervace =  rezervace.OrderBy(r => r.Datum).ToList();

            return View(rezervace);
        }

    }
}