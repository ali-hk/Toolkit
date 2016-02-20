using System.Collections.Generic;
using Toolkit.TestApp.Models;

namespace Toolkit.TestApp.Repositories
{
    public interface IAthleteRepository
    {
        IEnumerable<HockeyPlayer> GetHockeyPlayers(int count, int startIndex = 0);

        IEnumerable<Goaltender> GetGoaltenders(int count, int startIndex = 0);

        IEnumerable<Driver> GetDrivers(int count, int startIndex = 0);
    }
}
