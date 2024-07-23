using Microsoft.AspNetCore.Mvc;
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
            if(string.IsNullOrEmpty(CodiceFiscale))
            {
                return View("Index");
            }
            var prenotazioni = _prenotazioneDao.GetPrenotazioneByCF(CodiceFiscale);
            return View("Index", prenotazioni);
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
