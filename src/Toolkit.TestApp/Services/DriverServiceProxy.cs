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
    public class DriverServiceProxy : IDriverService
    {
        public IEnumerable<Driver> GetDrivers()
        {
            var content = File.ReadAllText(@"Data\Drivers.json");
            var drivers = JsonConvert.DeserializeObject<IEnumerable<Driver>>(content);
            return drivers.ToList();
        }
    }
}
