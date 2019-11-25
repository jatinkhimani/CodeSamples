using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirTraffic.DataAccess;

namespace AirTraffic.Repository
{
	public class AirTrafficVM
    {
        public List<AirCraft> WaitingToLandAirCrafts { get; set; }
        public List<AirCraft> WaitingToTakeOffAirCrafts { get; set; }
        public List<Runway> TaxingAirCrafts { get; set; }
    }
    public class AirTrafficRepository : IAirTrafficRepository
    {
        private readonly IAirCraftRepository _iAirCraftRepo;
        private readonly IRunwayRepository _iRunwayRepo;
        public AirTrafficRepository(IAirCraftRepository iAirCraftRepo
            , IRunwayRepository iRunwayRepo)
        {
            _iAirCraftRepo = iAirCraftRepo;
            _iRunwayRepo = iRunwayRepo;
        }

        public AirTrafficVM GetAirCraftsForATC()
        {
            var waitingToLandVal = (int)GeneralUtils.AirCraftStatus.WaitingToLand;
            var waitingToTakeOffVal = (int)GeneralUtils.AirCraftStatus.WaitingToTakeOff;

            var waitingToLandList = _iAirCraftRepo.SelectQuery(f => f.Status == waitingToLandVal).ToList();
            var waitingToTakeOffList = _iAirCraftRepo.SelectQuery(f => f.Status == waitingToTakeOffVal).ToList();
            var taxingList = _iRunwayRepo.SelectQuery(null, null, g => g.AirCraft).ToList();

            AirTrafficVM airTObj = new AirTrafficVM();
            airTObj.WaitingToLandAirCrafts = waitingToLandList;
            airTObj.WaitingToTakeOffAirCrafts = waitingToTakeOffList;
            airTObj.TaxingAirCrafts = taxingList;
            return airTObj;
        }
    }
}
