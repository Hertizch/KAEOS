using System.Net.NetworkInformation;
using KAEOS.Extensions;

namespace KAEOS.Models
{
    public class NetworkInterfaceName : ObservableObject
    {
        private string _description;
        private string _internalIp;
        private NetworkInterface _networkInterface;

        public string Description
        {
            get { return _description; }
            set { SetField(ref _description, value); }
        }

        public string InternalIp
        {
            get { return _internalIp; }
            set { SetField(ref _internalIp, value); }
        }

        public NetworkInterface NetworkInterface
        {
            get { return _networkInterface; }
            set { SetField(ref _networkInterface, value); }
        }
    }
}
