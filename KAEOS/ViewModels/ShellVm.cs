using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using KAEOS.Extensions;
using KAEOS.Utilities;

namespace KAEOS.ViewModels
{
    public class ShellVm : ObservableObject
    {
        public ShellVm()
        {
            var osVersion = WmiHelper.GetValueFromWmi("root\\CIMV2", "SELECT * FROM Win32_OperatingSystem", "Caption");
            var osArchitecture = WmiHelper.GetValueFromWmi("root\\CIMV2", "SELECT * FROM Win32_OperatingSystem", "OSArchitecture");
            var osBuildNumber = WmiHelper.GetValueFromWmi("root\\CIMV2", "SELECT * FROM Win32_OperatingSystem", "BuildNumber");

            var ohmVersionInfo = FileVersionInfo.GetVersionInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "OpenHardwareMonitorLib.dll"));

            Logger.WriteLine($"OS: {osVersion}, build: {osBuildNumber} ({osArchitecture})", true);
            Logger.WriteLine($"OpenHardwareMonitorLib.dll file version: {ohmVersionInfo.FileVersion}, product version: {ohmVersionInfo.ProductVersion}", true);
        }

        /*
         * Private fields
         */

        private RelayCommand _cmdCloseApplication;

        /*
         * Public fields
         */



        /*
         * Commands
         */

        public RelayCommand CmdCloseApplication
        {
            get
            {
                return _cmdCloseApplication ??
                       (_cmdCloseApplication = new RelayCommand(Execute_CloseApplication, p => true));
            }
        }

        /*
         * Methods
         */

        private static void Execute_CloseApplication(object obj)
        {
            Properties.Settings.Default.Save();

            Application.Current.Shutdown(0);
        }
    }
}
