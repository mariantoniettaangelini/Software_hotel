using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Software_hotel.DAO;
using Software_hotel.Models;
using System.Diagnostics;
using Software_hotel.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace Software_hotel.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPrenontazioneDao _prenotazioneDao;
        private readonly IAuthService _authService;

        public HomeController(ILogger<HomeController> logger, IAuthService authService, IPrenontazioneDao prenotazioneDao)
        {
            _logger = logger;
            _authService = authService;
            _prenotazioneDao = prenotazioneDao;
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string username, string password, string returnUrl = "/")
        {
            var user = _authService.ValidateUser(username, password);
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                if (!Url.IsLocalUrl(returnUrl))
                {
                    return RedirectToAction("Index", "Home");
                }

                return LocalRedirect(returnUrl);
            }

            ModelState.AddModelError(string.Empty, "Login non riuscito");
            return View();
        }

        public IActionResult Index()
        {
            var prenotazioni = _prenotazioneDao.GetAll();
            return View(prenotazioni);
        }

        public IActionResult ModificaPrenotazione(int id, decimal? importoDaSaldare = null)
        {
            var prenotazione = _prenotazioneDao.GetAll().FirstOrDefault(p => p.idPrenotazione == id);

            ViewBag.ServiziDisponibili = _prenotazioneDao.GetListaServizi()
                .Select(s => new SelectListItem
                {
                    Value = s.idServizio.ToString(),
                    Text = s.DescrizioneServizio
                }).ToList() ?? new List<SelectListItem>(); 

            ViewBag.Prezzi = new List<SelectListItem>
                {
                    new SelectListItem { Value = "10", Text = "€10.00" },
                    new SelectListItem { Value = "20", Text = "€20.00" },
                    new SelectListItem { Value = "30", Text = "€30.00" },
                };

                        ViewBag.Quantita = new List<SelectListItem>
                {
                    new SelectListItem { Value = "1", Text = "1" },
                    new SelectListItem { Value = "2", Text = "2" },
                    new SelectListItem { Value = "3", Text = "3" },
                };

            if (prenotazione == null)
            {
                return NotFound("Prenotazione non trovata.");
            }

            prenotazione.serviziPerPrenotazione = _prenotazioneDao.GetServiziPerPrenotazione(id).ToList();
            ViewBag.ImportoDaSaldare = importoDaSaldare ?? prenotazione.ImportoDaSaldare; 

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
            decimal importoDaSaldare = _prenotazioneDao.AggiungiServizio(idPrenotazione, servizio);
            return RedirectToAction("Index", new { id = idPrenotazione, importoDaSaldare = importoDaSaldare });
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
