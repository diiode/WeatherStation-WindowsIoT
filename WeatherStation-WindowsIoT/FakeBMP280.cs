using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherStation_WindowsIoT
{
    public class FakeBMP280: IBMP280
    {
        //Method to initialize the BMP280 sensor
        public async Task Initialize()
        {
            Debug.WriteLine("FakeBMP280::Initialize");
        }

        public async Task<float> ReadTemperature()
        {
            return RandomFloat(0, 50);
        }

        public async Task<float> ReadPressure()
        {
            return RandomFloat(990, 1030);
        }
        
        public async Task<float> ReadAltitude(float seaLevel)
        {
            return RandomFloat(0, 1000);
        }

        public async Task<float> ReadHumidity()
        {
            return RandomFloat(0, 100);
        }

        private float RandomFloat(float min, float max)
        {
            var rng = new Random();
            var result = min + (rng.NextDouble() * (max - min));
            return (float)result;
        }
    }
}
