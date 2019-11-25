using AirTraffic.Repository;
using AirTrafficApp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AirTrafficApp.Controllers
{
    public class AirTrafficController : Controller
    {
        private readonly IAirTrafficRepository _iAirTrafficRepo;
        private readonly IAirCraftRepository _iAirCraftRepo;
        private readonly IRunwayRepository _iRunwayRepo;
        public AirTrafficController(IAirTrafficRepository iAirTrafficRepo,
            IAirCraftRepository iAirCraftRepo,
            IRunwayRepository iRunwayRepo)
        {
            _iAirTrafficRepo = iAirTrafficRepo;
            _iAirCraftRepo = iAirCraftRepo;
            _iRunwayRepo = iRunwayRepo;
        }
        // GET: AirTraffic
        public ActionResult Index()
        {
            var airTrafficVM = _iAirTrafficRepo.GetAirCraftsForATC();
            if (Request.IsAjaxRequest())
            {
                return PartialView("_Dashboard", airTrafficVM);
            }
            return View(airTrafficVM);
        }

        public ActionResult Land(int airCraftId)
        {
            var airCraft = _iAirCraftRepo.GetAirCraft(airCraftId);
            var runwayObj = _iRunwayRepo.SelectQuery(f => f.AirCraftId == null).FirstOrDefault();
            if (runwayObj == null)
            {
                return ErrorMessage.GetCustomeMessage(false,"Please create runway to change the Air Craft Status or wait to free the runway.");
            }
            runwayObj.AirCraftId = airCraftId;
            runwayObj.NewStatus = (int)GeneralUtils.AirCraftStatus.WaitingToTakeOff;
            runwayObj.RunwayMessage = "Air Craft is Landing in....";

            _iRunwayRepo.UpdateRunway(runwayObj);

            airCraft.Status = (int)GeneralUtils.AirCraftStatus.Taxing;
            _iAirCraftRepo.UpdateAirCraft(airCraft);
            return ErrorMessage.GetCustomeMessage(true, "Air Craft Status updated successfully");
        }

        public ActionResult TakeOff(int airCraftId)
        {
            var airCraft = _iAirCraftRepo.GetAirCraft(airCraftId);
            var runwayObj = _iRunwayRepo.SelectQuery(f => f.AirCraftId == null).FirstOrDefault();
            if (runwayObj == null)
            {
                return ErrorMessage.GetCustomeMessage(false, "Please create runway to change the Air Craft Status or wait to free the runway.");
            }
            runwayObj.AirCraftId = airCraftId;
            runwayObj.NewStatus = (int)GeneralUtils.AirCraftStatus.WaitingToLand;
            runwayObj.RunwayMessage = "Air Craft is Taking off in....";

            _iRunwayRepo.UpdateRunway(runwayObj);

            airCraft.Status = (int)GeneralUtils.AirCraftStatus.Taxing;
            _iAirCraftRepo.UpdateAirCraft(airCraft);
            return ErrorMessage.GetCustomeMessage(true, "Air Craft Status updated successfully");
        }

        public ActionResult UpdateStatus(int id)
        {
            var airCraft = _iAirCraftRepo.GetAirCraft(id);
            var runwayObj = _iRunwayRepo.SelectQuery(f => f.AirCraftId == id).FirstOrDefault();

            airCraft.Status = runwayObj.NewStatus.Value;
            _iAirCraftRepo.UpdateAirCraft(airCraft);

            runwayObj.AirCraftId = null;
            runwayObj.NewStatus = null;
            runwayObj.RunwayMessage = string.Empty;

            _iRunwayRepo.UpdateRunway(runwayObj);
            
            return ErrorMessage.GetCustomeMessage(true, "Air Craft Status updated successfully");
        }
    }
}