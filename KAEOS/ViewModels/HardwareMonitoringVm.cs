using System;
using System.Linq;
using System.Timers;
using KAEOS.Extensions;
using KAEOS.Models;
using KAEOS.Properties;
using KAEOS.Utilities;
using OpenHardwareMonitor.Hardware;

namespace KAEOS.ViewModels
{
    public class HardwareMonitoringVm : ObservableObject
    {
        public HardwareMonitoringVm()
        {
            MonitoredHardware = new MonitoredHardware();

            if (CmdStartMonitoring.CanExecute(null))
                CmdStartMonitoring.Execute(null);
        }

        /*
         * Private fields
         */

        private MonitoredHardware _monitoredHardware;
        private Computer _computer;
        private Timer _timer;
        private RelayCommand _cmdStartMonitoring;
        private RelayCommand _cmdRestartMonitoring;
        private RelayCommand _cmdPrintReport;

        /*
         * Public fields
         */

        public MonitoredHardware MonitoredHardware
        {
            get { return _monitoredHardware; }
            set { SetField(ref _monitoredHardware, value); }
        }

        /*
         * Commands
         */

        public RelayCommand CmdStartMonitoring
        {
            get
            {
                return _cmdStartMonitoring ??
                       (_cmdStartMonitoring = new RelayCommand(Execute_StartMonitoring, p => true));
            }
        }

        public RelayCommand CmdRestartMonitoring
        {
            get
            {
                return _cmdRestartMonitoring ??
                       (_cmdRestartMonitoring = new RelayCommand(Execute_ReStartMonitoring, p => true));
            }
        }

        public RelayCommand CmdPrintReport
        {
            get
            {
                return _cmdPrintReport ??
                       (_cmdPrintReport = new RelayCommand(Execute_PrintReport, p => true));
            }
        }

        /*
         * Methods
         */

        private void Execute_StartMonitoring(object obj)
        {
            // Create computer instance
            _computer = new Computer
            {
                GPUEnabled = Settings.Default.ShowGpu,
                CPUEnabled = Settings.Default.ShowCpu,
                RAMEnabled = Settings.Default.ShowRam
            };

            Logger.WriteLine($"Created computer instance with parameters; GPU Enabled: '{_computer.GPUEnabled}', CPU Enabled: '{_computer.CPUEnabled}', RAM Enabled: '{_computer.RAMEnabled}'", true);

            // Start computer instance
            _computer.Open();

            // Iterate all hardware
            foreach (var hardware in _computer.Hardware.Where(hardware => hardware != null))
            {
                // Get hardware values
                hardware.Update();

                // Get GPU
                if (hardware.HardwareType.Equals(HardwareType.GpuNvidia) && Settings.Default.ShowGpu)
                {
                    var name = hardware.Name;
                    var load = GetSensorValue(hardware, SensorType.Load, "GPU Core");
                    var temperature = GetSensorValue(hardware, SensorType.Temperature, "GPU Core");
                    var fanSpeed = GetSensorValue(hardware, SensorType.Fan);

                    MonitoredHardware.GpuNvidias.Add(new GpuNvidia
                    {
                        Name = name,
                        Temperature = temperature,
                        Load = load,
                        FanSpeed = fanSpeed,
                        Hardware = hardware
                    });

                    Logger.WriteLine($"Added hardware: '{hardware.Name}', of type: '{hardware.HardwareType}', id: '{hardware.Identifier}'", true);
                }

                // Get CPU
                if (hardware.HardwareType.Equals(HardwareType.CPU) && Settings.Default.ShowCpu)
                {
                    var name = hardware.Name;
                    var load = GetSensorValue(hardware, SensorType.Load, "CPU Total");
                    var temperature = GetSensorValue(hardware, SensorType.Temperature, "CPU Package");
                    var fanSpeed = GetSensorValue(hardware, SensorType.Fan);

                    // Add to collection
                    MonitoredHardware.Cpus.Add(new Cpu
                    {
                        Name = name,
                        Temperature = temperature,
                        Load = load,
                        FanSpeed = fanSpeed,
                        Hardware = hardware
                    });

                    Logger.WriteLine($"Added hardware: '{hardware.Name}', of type: '{hardware.HardwareType}', id: '{hardware.Identifier}'", true);
                }

                // Get RAM
                if (hardware.HardwareType.Equals(HardwareType.RAM) && Settings.Default.ShowRam)
                {
                    var name = hardware.Name;
                    var load = GetSensorValue(hardware, SensorType.Load, "Memory");

                    // If vendor name could not be resolved, try to get from wmi
                    if (name.Equals("Generic Memory"))
                    {
                        var partNumber = WmiHelper.GetValueFromWmi("root\\CIMV2", "SELECT * FROM Win32_PhysicalMemory", "PartNumber");
                        if (partNumber != null)
                        {
                            name = GetMemoryModelNameFromPartNumber(partNumber);

                            Logger.WriteLine($"Resolved memory ({hardware.Name}) part number from WMI: '{partNumber}', model name: '{name}'", true);
                        }
                    }

                    // Add to collection
                    MonitoredHardware.Rams.Add(new Ram
                    {
                        Name = name,
                        Load = load,
                        Hardware = hardware
                    });

                    Logger.WriteLine($"Added hardware: '{name}', of type: '{hardware.HardwareType}', id: '{hardware.Identifier}'", true);
                }
            }

            // Create polling timer
            CreatePollingTimer();
        }

        private void Execute_ReStartMonitoring(object obj)
        {
            _computer?.Close();
            Logger.WriteLine("Closed running computer instance in HardwareMonitoringVm", true);

            _timer?.Dispose();
            Logger.WriteLine("Disposed running polling timer instance in HardwareMonitoringVm", true);

            if (MonitoredHardware != null)
            {
                MonitoredHardware = new MonitoredHardware();
                Logger.WriteLine("Reset monitored hardware class in HardwareMonitoringVm", true);
            }

            if (CmdStartMonitoring.CanExecute(null))
                CmdStartMonitoring.Execute(null);
        }

        private void CreatePollingTimer()
        {
            _timer = new Timer(1000);
            _timer.Elapsed += TimerOnElapsed;
            _timer.Start();

            Logger.WriteLine($"Created polling timer instance with interval: '{_timer.Interval}' in HardwareMonitoringVm", true);
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            // Update values -- Gpu Nvidia
            if (MonitoredHardware.GpuNvidias != null)
                foreach (var gpu in MonitoredHardware.GpuNvidias.Where(gpu => gpu.Hardware != null))
                {
                    gpu.Hardware.Update();

                    var load = GetSensorValue(gpu.Hardware, SensorType.Load, "GPU Core");
                    var temperature = GetSensorValue(gpu.Hardware, SensorType.Temperature, "GPU Core");
                    var fanSpeed = Settings.Default.ShowGpuFan ? GetSensorValue(gpu.Hardware, SensorType.Fan) : 0;

                    gpu.Load = load;
                    gpu.Temperature = temperature;
                    gpu.FanSpeed = fanSpeed;
                }

            // Update values -- Cpu
            if (MonitoredHardware.Cpus != null)
                foreach (var cpu in MonitoredHardware.Cpus.Where(cpu => cpu.Hardware != null))
                {
                    cpu.Hardware.Update();

                    var load = GetSensorValue(cpu.Hardware, SensorType.Load, "CPU Total");
                    var temperature = GetSensorValue(cpu.Hardware, SensorType.Temperature, "CPU Package");
                    var fanSpeed = Settings.Default.ShowCpuFan ? GetSensorValue(cpu.Hardware, SensorType.Fan) : 0;

                    cpu.Load = load;
                    cpu.Temperature = temperature;
                    cpu.FanSpeed = fanSpeed;
                }

            // Update values -- Ram
            if (MonitoredHardware.Rams != null)
                foreach (var ram in MonitoredHardware.Rams.Where(ram => ram.Hardware != null))
                {
                    ram.Hardware.Update();

                    var load = GetSensorValue(ram.Hardware, SensorType.Load, "Memory");

                    ram.Load = load;
                }
        }

        private void Execute_PrintReport(object obj)
        {
            var report = _computer?.GetReport();

            Logger.WriteLine(report, true);
        }

        private static float? GetSensorValue(IHardware hardware, SensorType sensorType, string sensorName = null)
        {
            float? results = null;

            // Check if sensor exists
            var hasValue = sensorName == null
                ? hardware.Sensors?.Any(x => x.SensorType.Equals(sensorType))
                : hardware.Sensors?.Any(x => x.SensorType.Equals(sensorType) && x.Name.Equals(sensorName));
            var b = !hasValue;
            if (b != null && (bool) b)
                return 0;

            try
            {
                results = sensorName == null
                    ? hardware.Sensors?.First(x => x.SensorType.Equals(sensorType)).Value
                    : hardware.Sensors?.First(x => x.SensorType.Equals(sensorType) && x.Name.Equals(sensorName)).Value;
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"Unable to get sensor value from hardware name: '{hardware.Name}', of type: '{sensorType}', sensor name: '{sensorName}' -- Exception: {ex.Message}", true);
            }

            return results ?? 0;
        }

        private static string GetMemoryModelNameFromPartNumber(string partNumber)
        {
            var results = "Generic Memory";

            switch (partNumber)
            {
                case "CMK16GX4M4A2800C16":
                    results = "Corsair Vengeance LPX";
                    break;
            }

            return results;
        }
    }
}
