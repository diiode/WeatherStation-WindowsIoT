using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherStation_WindowsIoT
{
    public interface IBMP280
    {
        Task Initialize();
        Task<float> ReadPressure();
        Task<float> ReadAltitude(float seaLevel);
        Task<float> ReadHumidity();
    }
}
