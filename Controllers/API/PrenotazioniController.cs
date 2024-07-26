using Microsoft.AspNetCore.Mvc;
using Software_hotel.DAO;
using Software_hotel.Models;

namespace Software_hotel.Controllers.API
{
        [Route("api/[controller]")]
        [ApiController]
    public class PrenotazioniController : Controller
    {
        private readonly IPrenontazioneDao _prenotazioneDao;

        public PrenotazioniController(IPrenontazioneDao prenotazioneDao)
        {
            _prenotazioneDao = prenotazioneDao;
        }

        [HttpGet("CodiceFiscale/{codiceFiscale}")]
        public async Task<IActionResult> GetPrenotazioniByCF(string codiceFiscale)
        {
            var prenotazioni = _prenotazioneDao.GetPrenotazioneByCF(codiceFiscale);
            if(prenotazioni == null)
            {
                return NotFound("Non ho trovato prenotazioni con questo CF");
            }

            return Ok(prenotazioni);
        }

        [HttpGet("PensioneCompleta")]
        public IActionResult GetPrenotazioniByPensione()
        {
            var prenotazioni = _prenotazioneDao.GetPrenotazioneByPensione("pensione completa");
            int numeroPrenotazioni = prenotazioni.Count();

            if (numeroPrenotazioni == 0) return NotFound("Non ho trovato prenotazioni");

            return Ok(numeroPrenotazioni);
        }
    }
}
