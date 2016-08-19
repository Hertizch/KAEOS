using System;
using System.Timers;
using KAEOS.Extensions;

namespace KAEOS.ViewModels
{
    public class DateTimeMonitoringVm : ObservableObject
    {
        public DateTimeMonitoringVm()
        {
            if (CmdStartMonitoring.CanExecute(null))
                CmdStartMonitoring.Execute(null);
        }

        /*
         * Private fields
         */

        private RelayCommand _cmdStartMonitoring;
        private DateTime _currentDateTime;

        /*
         * Public fields
         */

        public DateTime CurrentDateTime
        {
            get { return _currentDateTime; }
            set { SetField(ref _currentDateTime, value); }
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

        /*
         * Methods
         */

        private void Execute_StartMonitoring(object obj)
        {
            var timerCurrentDateTime = new Timer { Interval = 100 };
            timerCurrentDateTime.Elapsed += timerCurrentDateTime_Elapsed;
            timerCurrentDateTime.Start();
        }

        private void timerCurrentDateTime_Elapsed(object sender, ElapsedEventArgs e)
        {
            CurrentDateTime = DateTime.Now;
        }
    }
}
