using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.WindowsAzure.MobileServices;

namespace projectArduinoFirstTry
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //public static MobileServiceClient MobileService = new MobileServiceClient("http://localhost:58716");

        //Use this constructor instead after publishing to the cloud
        public static MobileServiceClient MobileService = new MobileServiceClient(
              "https://pharmacy.azure-mobile.net/",
              "GhbNgaNoYvJjGrBbXilBdBYBgfYYBw97"
        );
    }
}
