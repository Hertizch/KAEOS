using KAEOS.Extensions;

namespace KAEOS.Models
{
    public class MonitoredHardware : ObservableObject
    {
        public MonitoredHardware()
        {
            GpuNvidias = new MtObservableCollection<GpuNvidia>();
            GpuAtis = new MtObservableCollection<GpuAti>();
            Cpus = new MtObservableCollection<Cpu>();
            Rams = new MtObservableCollection<Ram>();
        }

        private MtObservableCollection<GpuNvidia> _gpuNvidias;
        private MtObservableCollection<GpuAti> _gpuAtis;
        private MtObservableCollection<Cpu> _cpus;
        private MtObservableCollection<Ram> _rams;

        public MtObservableCollection<GpuNvidia> GpuNvidias
        {
            get { return _gpuNvidias; }
            set { SetField(ref _gpuNvidias, value); }
        }

        public MtObservableCollection<GpuAti> GpuAtis
        {
            get { return _gpuAtis; }
            set { SetField(ref _gpuAtis, value); }
        }

        public MtObservableCollection<Cpu> Cpus
        {
            get { return _cpus; }
            set { SetField(ref _cpus, value); }
        }

        public MtObservableCollection<Ram> Rams
        {
            get { return _rams; }
            set { SetField(ref _rams, value); }
        }
    }
}
