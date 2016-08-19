using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Timers;
using KAEOS.Extensions;
using KAEOS.Models;
using KAEOS.Properties;
using KAEOS.Utilities;

namespace KAEOS.ViewModels
{
    public class NetworkMonitoringVm : ObservableObject
    {
        public NetworkMonitoringVm()
        {
            NetworkChange.NetworkAddressChanged += NetworkAddressChangedCallback;

            NetworkInterfaceNames = new MtObservableCollection<NetworkInterfaceName>();
            CurrentNetworkInterfaceName = new NetworkInterfaceName();

            if (CmdSetNetworkInterfaceName.CanExecute(null))
                CmdSetNetworkInterfaceName.Execute(null);

            if (CmdStartMonitoring.CanExecute(null))
                CmdStartMonitoring.Execute(null);
        }

        private void NetworkAddressChangedCallback(object sender, EventArgs e)
        {
            if (NetworkInterfaceNames.Count > 0)
            {
                foreach (var networkInterfaceName in NetworkInterfaceNames.Where(networkInterfaceName => networkInterfaceName.Description.Equals(CurrentNetworkInterfaceName.Description)))
                {
                    var dnsHostEntry = Dns.GetHostEntry(Dns.GetHostName());
                    networkInterfaceName.InternalIp =
                        dnsHostEntry.AddressList.First(x => x.AddressFamily.Equals(AddressFamily.InterNetwork)).ToString();
                }
            }
        }

        /*
         * Private fields
         */

        private NetworkInterfaceName _currentNetworkInterfaceName;
        private MtObservableCollection<NetworkInterfaceName> _networkInterfaceNames; 
        private float? _networkBytesSent;
        private float? _networkBytesRecieved;
        private Timer _timer;
        private RelayCommand _cmdSetNetworkInterfaceName;
        private RelayCommand _cmdStartMonitoring;

        /*
         * Public fields
         */

        public NetworkInterfaceName CurrentNetworkInterfaceName
        {
            get { return _currentNetworkInterfaceName; }
            set { SetField(ref _currentNetworkInterfaceName, value); }
        }

        public MtObservableCollection<NetworkInterfaceName> NetworkInterfaceNames
        {
            get { return _networkInterfaceNames; }
            set { SetField(ref _networkInterfaceNames, value); }
        }

        public float? NetworkBytesSent
        {
            get { return _networkBytesSent; }
            set { SetField(ref _networkBytesSent, value); }
        }

        public float? NetworkBytesRecieved
        {
            get { return _networkBytesRecieved; }
            set { SetField(ref _networkBytesRecieved, value); }
        }

        /*
         * Commands
         */

        public RelayCommand CmdSetNetworkInterfaceName
        {
            get
            {
                return _cmdSetNetworkInterfaceName ??
                       (_cmdSetNetworkInterfaceName = new RelayCommand(Execute_SetNetworkInterfaceName, p => true));
            }
        }

        public RelayCommand CmdStartMonitoring
        {
            get
            {
                return _cmdStartMonitoring ??
                       (_cmdStartMonitoring = new RelayCommand(Execute_StartMonitoring, p => true));
            }
        }

        /*
         * Methods
         */

        private void Execute_SetNetworkInterfaceName(object obj)
        {
            // Get all network interfaces and filter out unwanted
            var networkInterfaces =
                NetworkInterface.GetAllNetworkInterfaces().
                    Where(
                        x =>
                            x.OperationalStatus == OperationalStatus.Up &&
                            x.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                            x.NetworkInterfaceType != NetworkInterfaceType.Tunnel);

            // Loop the results
            foreach (var networkInterface in networkInterfaces)
            {
                var nic = networkInterface.Description;

                // PerformanceCounter class uses different characters than the GetAllNetworkInterfaces class, so replace them
                nic = nic.Replace("\\", "_");
                nic = nic.Replace("/", "_");
                nic = nic.Replace("(", "[");
                nic = nic.Replace(")", "]");
                nic = nic.Replace("#", "_");

                // Add it to the collection
                NetworkInterfaceNames.Add(new NetworkInterfaceName
                {
                    Description = nic,
                    NetworkInterface = networkInterface
                });

                Logger.WriteLine($"Added network interface: '{nic}', of type: '{networkInterface.NetworkInterfaceType}', id: '{networkInterface.Id}'", true);

                if (NetworkInterfaceNames.Count > 0)
                {
                    foreach (var networkInterfaceName in NetworkInterfaceNames.Where(networkInterfaceName => networkInterfaceName.Description.Equals(CurrentNetworkInterfaceName.Description)))
                    {
                        var dnsHostEntry = Dns.GetHostEntry(Dns.GetHostName());
                        networkInterfaceName.InternalIp =
                            dnsHostEntry.AddressList.First(x => x.AddressFamily.Equals(AddressFamily.InterNetwork)).ToString();
                    }
                }
            }

            // Set the first nic as current adapter, if any
            if (NetworkInterfaceNames.Count > 0)
            {
                CurrentNetworkInterfaceName.Description = NetworkInterfaceNames.First().Description;

                Logger.WriteLine($"Current network interface set to: '{CurrentNetworkInterfaceName.Description}'", true);
            }
        }

        private void Execute_StartMonitoring(object obj)
        {
            NetworkBytesSent = Settings.Default.ShowUpload
                ? Counters.GetNetworkBytesSentValue(CurrentNetworkInterfaceName.Description)
                : 0;

            NetworkBytesRecieved = Settings.Default.ShowDownload
                ? Counters.GetNetworkBytesRecievedValue(CurrentNetworkInterfaceName.Description)
                : 0;

            // Create polling timer
            CreatePollingTimer();
        }

        private void CreatePollingTimer()
        {
            _timer = new Timer(1000);
            _timer.Elapsed += TimerOnElapsed;
            _timer.Start();

            Logger.WriteLine($"Created polling timer instance with interval: '{_timer.Interval}' in NetworkMonitoringVm", true);
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            NetworkBytesSent = Settings.Default.ShowUpload
                ? Counters.GetNetworkBytesSentValue(CurrentNetworkInterfaceName.Description)
                : 0;

            NetworkBytesRecieved = Settings.Default.ShowDownload
                ? Counters.GetNetworkBytesRecievedValue(CurrentNetworkInterfaceName.Description)
                : 0;
        }
    }
}
