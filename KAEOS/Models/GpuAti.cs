using KAEOS.Extensions;
using OpenHardwareMonitor.Hardware;

namespace KAEOS.Models
{
    public class GpuAti : ObservableObject
    {
        public ISensor GpuAtiSensor { get; set; }
    }
}
