using AirTraffic.DataAccess;
using AirTraffic.Repository;
using AirTrafficApp.Common;
using RepositoryCore;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AirTrafficApp.Controllers
{
    public class AirCraftController : Controller
    {
        private readonly IAirCraftRepository _iAirCraftRepo;
        public AirCraftController(IAirCraftRepository iAirCraftRepo)
        {
            _iAirCraftRepo = iAirCraftRepo;
        }
        // GET: AirCraft
        public ActionResult Index()
        {
            var airCrafts = _iAirCraftRepo.GetAllAirCrafts();
            if (Request.IsAjaxRequest())
            {
                return PartialView("_AirCraftList", airCrafts);
            }
            return View(airCrafts);
        }
        
        // GET: AirCraft/Create
        public ActionResult Create()
        {
            var enumValues = Enum.GetValues(typeof(GeneralUtils.AirCraftStatus)).Cast<GeneralUtils.AirCraftStatus>().ToList();
            List<KeyValuePair<string, int>> keyValList = new List<KeyValuePair<string, int>>();
            for (int i = 0; i < enumValues.Count; i++)
            {
                keyValList.Add(new KeyValuePair<string, int>(enumValues[i].ToString(), i + 1));
            }
            ViewBag.AirCraftStatus = new SelectList(keyValList, "Value", "Key");
            return PartialView("_Create", new AirCraft());
        }

        // POST: AirCraft/Create
        [HttpPost]
        public ActionResult Create(AirCraft airCraft)
        {
            try
            {
                airCraft.Status = (int)GeneralUtils.AirCraftStatus.WaitingToTakeOff;
                airCraft.FuelPercentage = 100;
                _iAirCraftRepo.AddAirCraft(airCraft);
                return ErrorMessage.GetCustomeMessage(true, "Air Craft Created Successfully");
            }
            catch (UniqueKeyException)
            {
                return ErrorMessage.GetGenericCreatedFailed("AirCraftId");
            }
            catch (DbEntityValidationException)
            {
                return ErrorMessage.GetGenericModalFailed();
            }
        }
        
        public ActionResult Edit(int id)
        {
            if (id == 0)
            {
                return ErrorMessage.GetGenericUnknownFailed();
            }
            var airCraft = _iAirCraftRepo.GetAirCraft(id);
            return PartialView("_Edit", airCraft);
        }

        // POST: AirCraft/Edit/5
        [HttpPost]
        public ActionResult Edit(AirCraft airCraft)
        {
            try
            {
                _iAirCraftRepo.UpdateAirCraft(airCraft);
                return ErrorMessage.GetCustomeMessage(true, "AirCraft updated Successfully");
            }
            catch (UniqueKeyException)
            {
                return ErrorMessage.GetGenericEditFailed("AirCraftId");
            }
            catch (DbEntityValidationException)
            {
                return ErrorMessage.GetGenericModalFailed();
            }
        }

        // GET: AirCraft/Delete/5
        public ActionResult Delete(int id)
        {
            var airCraft = _iAirCraftRepo.GetAirCraft(id);
            return PartialView("_Delete", airCraft);
        }

        // POST: AirCraft/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfimed(int id)
        {
            try
            {
                _iAirCraftRepo.DeleteAirCraft(id);
                return ErrorMessage.GetCustomeMessage(true,"AirCraft Deleted Successfully");
            }
            catch(ForiegnKeyException)
            {
                return ErrorMessage.GetGenericDeleteFailed("AirCraft");
            }
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            var empId = 0;
            if (file != null)
            {
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~/Images/Employee"), empId + fileName);
                file.SaveAs(path);
            }
            return RedirectToAction("Index");
            
        }
    }
}
