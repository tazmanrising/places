using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calibrus.ClearviewPortal.DataAccess.Entities
{
    public class TrackAgent
    {
        public int AgentId { get; set; }
        public geolocation Geolocation { get; set; }

    }

    public class geolocation
    {
        public string lat { get; set; }
        public string lng { get; set; }
    }
}
