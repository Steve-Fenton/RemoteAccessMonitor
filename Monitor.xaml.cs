using System;
using System.Diagnostics;
using System.Management;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;

namespace RemoteAccessScanner
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Monitor : Window
    {
        private Timer _timer;
        private ObservableCollection<RemoteAccessEvent> _events;
        private ObservableCollection<ConnectedUser> _connectedUsers;
        private ICheckingProcess _checkingProcess;

        const string StopState = "Stop";
        const string StartState = "Start";
        const string MonitoringState = "Monitoring";
        const string NotMonitoringState = "Stopped";
        const string ErrorState = "Remote Session Detected";

        public Monitor()
        {
            _checkingProcess = new CheckingProcess(new SystemProcessProvider());
            _events = new ObservableCollection<RemoteAccessEvent>();
            _connectedUsers = new ObservableCollection<ConnectedUser>();
            InitializeComponent();
            MinimizeToTray.Enable(this);
            SetUpCheck();
        }

        /// <summary>
        /// Provides a handle for the XAML file to get hold of the private _events collection
        /// </summary>
        public ObservableCollection<RemoteAccessEvent> EventCollection
        {
            get { return _events; }
        }

        /// <summary>
        /// Provides a handle for the XAML file to get hold of the private _connectedUsers collection
        /// </summary>
        public ObservableCollection<ConnectedUser> ConnectionCollection
        {
            get { return _connectedUsers; }
        }

        /// <summary>
        /// Sets up the timer and starts it off
        /// </summary>
        private void SetUpCheck()
        {
            _timer = new Timer();
            _timer.Elapsed += new ElapsedEventHandler(CheckRemoteAccess);
            _timer.Interval = 10000;

            Working.GIFSource = "Working.gif";

            ProcessStartStop(StartState);
        }

        /// <summary>
        /// Handles each timer "tick"
        /// </summary>
        private void CheckRemoteAccess(object source, ElapsedEventArgs e)
        {
            _timer.Stop();
            CheckRemoteAccessResponse result = _checkingProcess.CheckRemoteAccess(_connectedUsers);
            MainGrid.Dispatcher.BeginInvoke(DispatcherPriority.Render, new VoidCollectionDelegate(ProcessConnectedUserList), result.ConnectedUsers);
            // Process each message
            foreach (RemoteAccessEvent remoteAccessEvent in result.RemoteAccessEvents)
            {
                MainGrid.Dispatcher.BeginInvoke(DispatcherPriority.Render, new VoidDelegate(ProcessRemoteAccessEvent), remoteAccessEvent);
            }
            _timer.Start();
        }

        private delegate void VoidCollectionDelegate(Collection<ConnectedUser> stringCollection);
        /// <summary>
        /// Handles conversion of user list into observable collection
        /// </summary>
        /// <param name="remoteAccessEvent"></param>
        private void ProcessConnectedUserList(Collection<ConnectedUser> stringCollection)
        {
            _connectedUsers.Clear();
            foreach (ConnectedUser user in stringCollection)
            {
                _connectedUsers.Add(user);
            }
        }

        private delegate void VoidDelegate(RemoteAccessEvent remoteAccessEvent);
        /// <summary>
        /// Handles any remote access events
        /// </summary>
        /// <param name="processUser"></param>
        private void ProcessRemoteAccessEvent(RemoteAccessEvent remoteAccessEvent)
        {
            _events.Add(remoteAccessEvent);

            this.Focus();

            WarningNote.Text = string.Format("{0} {1}", ErrorState, remoteAccessEvent.UserName);
            SetWarning();
            this.WindowState = WindowState.Normal;
        }

        /// <summary>
        /// Used to stop and start the monitor's scanning
        /// </summary>
        /// <param name="currentState"></param>
        private void ProcessStartStop(string currentState)
        {
            switch (currentState)
            {
                case StartState:
                    _timer.Stop();
                    _timer.Start();
                    ClearWarning();
                    break;
                case StopState:
                    _timer.Stop();
                    SetStopped();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Puts the UI into the errored state
        /// </summary>
        private void SetStopped()
        {
            StartStop.Content = StartState;
            WarningNote.Text = NotMonitoringState;
            Working.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Put the UI into the warning state
        /// </summary>
        private void SetWarning()
        {
            WarningNote.Foreground = Brushes.White;
            MainGrid.Background = Brushes.Red;
            ClearAlert.Visibility = Visibility.Visible;

            Uri iconUri = new Uri("pack://application:,,,/RemoteAlertIcon.ico", UriKind.RelativeOrAbsolute);
            MonitorWindow.Icon = BitmapFrame.Create(iconUri);
        }

        /// <summary>
        /// Resets the UI back to normal state
        /// </summary>
        private void ClearWarning()
        {
            StartStop.Content = StopState;
            WarningNote.Text = MonitoringState;
            WarningNote.Foreground = Brushes.White;
            MainGrid.Background = Brushes.Black;
            Working.Visibility = Visibility.Visible;
            ClearAlert.Visibility = Visibility.Collapsed;

            Uri iconUri = new Uri("pack://application:,,,/RemoteAccessIcon.ico", UriKind.RelativeOrAbsolute);
            MonitorWindow.Icon = BitmapFrame.Create(iconUri);
        }

        /// <summary>
        /// Handles the start / stop button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartStop_Click(object sender, RoutedEventArgs e)
        {
            Button originator = (Button)sender;
            string currentOperation = (string)originator.Content;
            ProcessStartStop(currentOperation);
        }

        /// <summary>
        /// Handles the Clear Alert button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearAlert_Click(object sender, RoutedEventArgs e)
        {
            ClearWarning();
        }

        /// <summary>
        /// Handles the Clear Alert button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearEvents_Click(object sender, RoutedEventArgs e)
        {
            _events.Clear();
        }

    }
}
