using RoadStatusClient.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RoadStatusClient.Service
{
    public interface IRoadStatusService
    {
        Task<IEnumerable<RoadStatusDto>> GetRoadStatusFromApiAsync(string roadIds);
        Task RetreiveRoadStatus(string roadIds);
    }
}
