using System;
using System.Windows;

namespace KAEOS.Controls
{
    /// <summary>
    /// Interaction logic for GaugeContainer.xaml
    /// </summary>
    public partial class GaugeContainer
    {
        public GaugeContainer()
        {
            InitializeComponent();

            PercentageFormatter = x => Math.Truncate(x) + "%";
            TemperatureFormatter = x => Math.Truncate(x) + "°C";
            RpmFormatter = x => Math.Truncate(x) + "RPM";
        }

        public string HardwareName
        {
            get { return (string)GetValue(HardwareNameProperty); }
            set { SetValue(HardwareNameProperty, value); }
        }

        public float? Load
        {
            get { return (float?)GetValue(LoadProperty); }
            set { SetValue(LoadProperty, value); }
        }

        public float? Temperature
        {
            get { return (float?)GetValue(TemperatureProperty); }
            set { SetValue(TemperatureProperty, value); }
        }

        public float? FanSpeed
        {
            get { return (float?)GetValue(FanSpeedProperty); }
            set { SetValue(FanSpeedProperty, value); }
        }

        public bool FanSpeedVisibility
        {
            get { return (bool)GetValue(FanSpeedVisibilityProperty); }
            set { SetValue(FanSpeedVisibilityProperty, value); }
        }

        public Func<double, string> PercentageFormatter { get; set; }

        public Func<double, string> TemperatureFormatter { get; set; }

        public Func<double, string> RpmFormatter { get; set; }

        public static readonly DependencyProperty HardwareNameProperty = DependencyProperty.Register("HardwareName", typeof(string), typeof(GaugeContainer), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty LoadProperty = DependencyProperty.Register("Load", typeof(float?), typeof(GaugeContainer), new PropertyMetadata(default(float?)));

        public static readonly DependencyProperty TemperatureProperty = DependencyProperty.Register("Temperature", typeof(float?), typeof(GaugeContainer), new PropertyMetadata(default(float?)));

        public static readonly DependencyProperty FanSpeedProperty = DependencyProperty.Register("FanSpeed", typeof(float?), typeof(GaugeContainer), new PropertyMetadata(default(float?)));

        public static readonly DependencyProperty FanSpeedVisibilityProperty = DependencyProperty.Register("FanSpeedVisibility", typeof(bool), typeof(GaugeContainer), new PropertyMetadata(default(bool)));
    }
}
