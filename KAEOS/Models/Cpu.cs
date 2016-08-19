using KAEOS.Extensions;
using OpenHardwareMonitor.Hardware;

namespace KAEOS.Models
{
    public class Cpu : ObservableObject
    {
        private string _name;
        private float? _temperature;
        private float? _load;
        private float? _fanSpeed;

        public IHardware Hardware { get; set; }

        public string Name
        {
            get { return _name; }
            set { SetField(ref _name, value); }
        }

        public float? Temperature
        {
            get { return _temperature; }
            set { SetField(ref _temperature, value); }
        }

        public float? Load
        {
            get { return _load; }
            set { SetField(ref _load, value); }
        }

        public float? FanSpeed
        {
            get { return _fanSpeed; }
            set { SetField(ref _fanSpeed, value); }
        }
    }
}
