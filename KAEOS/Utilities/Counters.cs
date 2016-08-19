using System.Diagnostics;
using System.Threading;

namespace KAEOS.Utilities
{
    public static class Counters
    {
        public static float? GetCpuUsageValue()
        {
            float output;

            using (var pc = new PerformanceCounter
            {
                CategoryName = "Processor",
                CounterName = "% Processor Time",
                InstanceName = "_Total"
            })
            {
                pc.NextValue();
                Thread.Sleep(1000);
                output = pc.NextValue();
            }

            return output;
        }

        public static float? GetNetworkBytesSentValue(string networkInterfaceName)
        {
            float output;

            using (var pc = new PerformanceCounter("Network Interface", "Bytes Sent/sec", networkInterfaceName))
            {
                pc.NextValue();
                Thread.Sleep(1000);
                output = pc.NextValue();
            }

            return output;
        }

        public static float? GetNetworkBytesRecievedValue(string networkInterfaceName)
        {
            float output;

            using (var pc = new PerformanceCounter("Network Interface", "Bytes Received/sec", networkInterfaceName))
            {
                pc.NextValue();
                Thread.Sleep(1000);
                output = pc.NextValue();
            }

            return output;
        }
    }
}
