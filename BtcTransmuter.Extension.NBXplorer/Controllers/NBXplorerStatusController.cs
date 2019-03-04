using System.Threading.Tasks;
using BtcTransmuter.Extension.NBXplorer.HostedServices;
using BtcTransmuter.Extension.NBXplorer.Models;
using BtcTransmuter.Extension.NBXplorer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BtcTransmuter.Extension.NBXplorer.Controllers
{
    [Route("nbxplorer-plugin/status")]
    public class NBXplorerStatusController : Controller
    {
        private readonly NBXplorerSummaryProvider _nbXplorerSummaryProvider;

        public NBXplorerStatusController(NBXplorerSummaryProvider nbXplorerSummaryProvider)
        {
            _nbXplorerSummaryProvider = nbXplorerSummaryProvider;
        }

        public IActionResult GetSummaries()
        {
            return View(new NBXplorerSummariesViewModel()
            {
                Summaries = _nbXplorerSummaryProvider.GetSummaries()
            });
        }
    }
}