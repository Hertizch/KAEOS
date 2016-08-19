using KAEOS.Extensions;
using OpenHardwareMonitor.Hardware;

namespace KAEOS.Models
{
    public class Ram : ObservableObject
    {
        private string _name;
        private float? _load;

        public IHardware Hardware { get; set; }

        public string Name
        {
            get { return _name; }
            set { SetField(ref _name, value); }
        }

        public float? Load
        {
            get { return _load; }
            set { SetField(ref _load, value); }
        }
    }
}
