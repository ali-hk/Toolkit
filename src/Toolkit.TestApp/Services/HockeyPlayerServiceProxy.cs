using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.TestApp.Models;

namespace Toolkit.TestApp.Services
{
    public class HockeyPlayerServiceProxy : IHockeyPlayerService
    {
        public IEnumerable<HockeyPlayer> GetHockeyPlayers()
        {
            var content = File.ReadAllText(@"Data\HockeyPlayers.json");
            var hockeyPlayers = JsonConvert.DeserializeObject<IEnumerable<HockeyPlayer>>(content);
            return hockeyPlayers;
        }

        public IEnumerable<Goaltender> GetGoaltenders()
        {
            var content = File.ReadAllText(@"Data\Goaltenders.json");
            var goaltenders = JsonConvert.DeserializeObject<IEnumerable<Goaltender>>(content);
            return goaltenders;
        }
    }
}
