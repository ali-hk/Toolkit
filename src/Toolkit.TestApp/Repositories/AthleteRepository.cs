using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Collections.Extensions;
using Toolkit.TestApp.Models;
using Toolkit.TestApp.Services;

namespace Toolkit.TestApp.Repositories
{
    public class AthleteRepository : IAthleteRepository
    {
        private IHockeyPlayerService _hockeyPlayerService;
        private IDriverService _driverService;
        private List<Driver> _drivers;
        private List<HockeyPlayer> _hockeyPlayers;
        private List<Goaltender> _goaltenders;

        public AthleteRepository(IHockeyPlayerService hockeyPlayerService, IDriverService driverService)
        {
            _hockeyPlayerService = hockeyPlayerService;
            _driverService = driverService;
            _drivers = new List<Driver>();
            _hockeyPlayers = new List<HockeyPlayer>();
            _goaltenders = new List<Goaltender>();
        }

        public IEnumerable<HockeyPlayer> GetHockeyPlayers(int count, int startIndex = 0)
        {
            if (_hockeyPlayers.IsEmpty())
            {
                _hockeyPlayers = _hockeyPlayerService.GetHockeyPlayers().ToList();
            }

            var actualCount = _hockeyPlayers.Count();
            var result = new List<HockeyPlayer>();

            // Generate dupes since we don't have a large data set
            for (int i = startIndex % actualCount; i < count + startIndex; i++)
            {
                result.Add(_hockeyPlayers[i % actualCount]);
            }

            return result;
        }

        public IEnumerable<Goaltender> GetGoaltenders(int count, int startIndex = 0)
        {
            if (_goaltenders.IsEmpty())
            {
                _goaltenders = _hockeyPlayerService.GetGoaltenders().ToList();
            }

            var actualCount = _goaltenders.Count();
            var result = new List<Goaltender>();

            // Generate dupes since we don't have a large data set
            for (int i = startIndex % actualCount; i < count + startIndex; i++)
            {
                result.Add(_goaltenders[i % actualCount]);
            }

            return result;
        }

        public IEnumerable<Driver> GetDrivers(int count, int startIndex = 0)
        {
            if (_drivers.IsEmpty())
            {
                _drivers = _driverService.GetDrivers().ToList();
            }

            var actualCount = _drivers.Count();
            var result = new List<Driver>();

            // Generate dupes since we don't have a large data set
            for (int i = startIndex % actualCount; i < count + startIndex; i++)
            {
                result.Add(_drivers[i % actualCount]);
            }

            return result;
        }
    }
}
