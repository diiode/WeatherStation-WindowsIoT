using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Net.Http;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Background;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WeatherStation_WindowsIoT
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        IBMP280 bmp280;

        DispatcherTimer dispatcherTimer;
        DateTimeOffset startTime;
        DateTimeOffset lastTime;
        DateTimeOffset stopTime;
        int timesTicked = 1;
        int timesToTick = 2;

        public void DispatcherTimerSetup()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 2);
            startTime = DateTimeOffset.Now;
            lastTime = startTime;
            dispatcherTimer.Start();
        }

        async void dispatcherTimer_Tick(object sender, object e)
        {
            DateTimeOffset time = DateTimeOffset.Now;
            TimeSpan span = time - lastTime;
            lastTime = time;
            timesTicked++;
            if (timesTicked > timesToTick)
            {
                float temp = 0;
                float pressure = 0;
                float altitude = 0;
                float humidity = 0;
                const float seaLevelPressure = 1013.25f;
                temp = await bmp280.ReadTemperature();
                pressure = await bmp280.ReadPressure();
                altitude = await bmp280.ReadAltitude(seaLevelPressure);
                humidity = await bmp280.ReadHumidity();

                //Write the values to your debug console
                Debug.WriteLine("Temperature: " + temp.ToString() + " deg C");
                Debug.WriteLine("Pressure: " + pressure.ToString() + " Pa");
                Debug.WriteLine("Altitude: " + altitude.ToString() + " m");
                Debug.WriteLine("Humidity: " + humidity.ToString() + "%");

                TemperatureText.Text = temp.ToString() + " °C";
                PressureText.Text = pressure.ToString() + " hPa";
                HumidityText.Text = pressure.ToString() + "%";

                stopTime = time;

                span = stopTime - startTime;
            }
        }

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            Debug.WriteLine("MainPage::OnNavigatedTo");


            MakePinWebAPICall();

            try
            {
                //Create a new object for our barometric sensor class
                bmp280 = new FakeBMP280();
                //Initialize the sensor
                await bmp280.Initialize();

                ////Create variables to store the sensor data: temperature, pressure and altitude. 
                ////Initialize them to 0.
                //float temp = 0;
                //float pressure = 0;
                //float altitude = 0;

                //float humidity = 0;

                ////Create a constant for pressure at sea level. 
                ////This is based on your local sea level pressure (Unit: Hectopascal)
                //const float seaLevelPressure = 1013.25f;

                ////Read 10 samples of the data
                //for (int i = 0; i < 10; i++)
                //{
                //    temp = await bmp280.ReadTemperature();
                //    pressure = await bmp280.ReadPressure();
                //    altitude = await bmp280.ReadAltitude(seaLevelPressure);
                //    humidity = await bmp280.ReadHumidity();

                //    //Write the values to your debug console
                //    Debug.WriteLine("Temperature: " + temp.ToString() + " deg C");
                //    Debug.WriteLine("Pressure: " + pressure.ToString() + " Pa");
                //    Debug.WriteLine("Altitude: " + altitude.ToString() + " m");
                //    Debug.WriteLine("Humidity: " + humidity.ToString() + " %");
                //}

                DispatcherTimerSetup();


                //this.RegisterBackgroundTask();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        //private async void RegisterBackgroundTask()
        //{
        //    var backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
        //    if (backgroundAccessStatus == BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity ||
        //        backgroundAccessStatus == BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity)
        //    {
        //        foreach (var task in BackgroundTaskRegistration.AllTasks)
        //        {
        //            if (task.Value.Name == taskName)
        //            {
        //                task.Value.Unregister(true);
        //            }
        //        }

        //        BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder();
        //        taskBuilder.Name = "DashBoardUpdateBackgroundTask";
        //        taskBuilder.TaskEntryPoint = "WeatherStation_WindowsIoT.DashBoardUpdateBackgroundTask";
        //        taskBuilder.SetTrigger(new TimeTrigger(15, false));
        //        var registration = taskBuilder.Register();
        //    }
        //}

        public void MakePinWebAPICall()
        {
            try
            {
                HttpClient client = new HttpClient();

                // Comment this line to opt out of the pin map.
                client.GetStringAsync("http://adafruitsample.azurewebsites.net/api?Lesson=203");
            }
            catch (Exception e)
            {
                Debug.WriteLine("Web call failed: " + e.Message);
            }
        }
    }

    //public sealed class DashBoardUpdateBackgroundTask : IBackgroundTask
    //{
    //    public void Run(IBackgroundTaskInstance taskInstance)
    //    {
    //        BackgroundTaskDeferral deferral = taskInstance.GetDeferral();
            


    //        deferral.Complete();
    //    }
    //}
}
