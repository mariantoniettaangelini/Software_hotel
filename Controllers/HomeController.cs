using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Software_hotel.DAO;
using Software_hotel.Models;
using System.Diagnostics;

namespace Software_hotel.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPrenontazioneDao _prenotazioneDao;

        public HomeController(ILogger<HomeController> logger, IPrenontazioneDao prenontazioneDao)
        {
            _logger = logger;
            _prenotazioneDao = prenontazioneDao;
        }

        public IActionResult Index()
        {
            var prenotazioni = _prenotazioneDao.GetAll();
            return View(prenotazioni);
        }

        public IActionResult CercaCliente(string CodiceFiscale)
        {
            if (string.IsNullOrEmpty(CodiceFiscale))
            {
                return View("Index");
            }
            var prenotazioni = _prenotazioneDao.GetPrenotazioneByCF(CodiceFiscale);
            return View("Index", prenotazioni);
        }

        public IActionResult PrenotazionePensioneCompleta()
        {
            var prenotazioni = _prenotazioneDao.GetPrenotazioneByPensione("pensione completa");
            return View("PensioneCompleta", prenotazioni);
        }

        public IActionResult ModificaPrenotazione(int id)
        {
            var prenotazione = _prenotazioneDao.GetAll().FirstOrDefault(p => p.idPrenotazione == id);
            if (prenotazione == null)
            {
                return NotFound();
            }
            prenotazione.serviziPerPrenotazione = _prenotazioneDao.GetServiziPerPrenotazione(id).ToList();

            var serviziDisponibili = _prenotazioneDao.GetListaServizi()
                                    .Select(s => new
                                    {
                                        idServizio = s.idServizio,
                                        DescrizioneServizio = s.DescrizioneServizio
                                    }).ToList();

            ViewBag.ServiziDisponibili = serviziDisponibili;

            var prezzi = new List<SelectListItem>
        {
            new SelectListItem { Value = "10", Text = "€10.00" },
            new SelectListItem { Value = "20", Text = "€20.00" },
            new SelectListItem { Value = "30", Text = "€30.00" },
        };
                ViewBag.Prezzi = prezzi;

                var quantita = new List<SelectListItem>
        {
            new SelectListItem { Value = "1", Text = "1" },
            new SelectListItem { Value = "2", Text = "2" },
            new SelectListItem { Value = "3", Text = "3" },
        };
                ViewBag.Quantita = quantita;

            return View(prenotazione);
        }

        [HttpPost]
        public IActionResult AggiungiServizio(int idPrenotazione, int idServizio, decimal prezzo, int quantita)
        {
            var servizio = new ServizioPerPrenotazione
            {
                idServizio = idServizio,
                Prezzo = prezzo,
                Quantita = quantita
            };
            _prenotazioneDao.AggiungiServizio(idPrenotazione, servizio);
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
