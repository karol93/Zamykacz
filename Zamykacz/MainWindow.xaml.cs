using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Zamykacz
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window
   {
      private int _shortcutHour = 0;
      private DispatcherTimer _timer;
      private TimeSpan _time;
      public MainWindow()
      {
         InitializeComponent();
      }

      private void Cancel_Click(object sender, RoutedEventArgs e)
      {
         _timer.Stop();
         Window.Background = Brushes.White;
         StartGroupBox.Visibility = Visibility.Visible;
         CancelGroupBox.Visibility = Visibility.Hidden;
         EnableAllControls();
      }

      private void Start_Click(object sender, RoutedEventArgs e)
      {
         if (CloseAtTimePicker.Value == null && CloseForTimePicker.Value == null && !(bool)OneHourShortcut.IsChecked && !(bool)TwoHourShortcut.IsChecked && !(bool)ThreeHourShortcut.IsChecked)
         {
            MessageBox.Show("Nie ustawiono za ile lub o której godzinie ma zostać wyłączony system.", "Ups...");
            return;
         }
         if (CloseAtTimePicker.Value != null)
         {
            if (DateTime.Now >= CloseAtTimePicker.Value.Value)
            {
               MessageBox.Show("Czas kiedy system ma zostać zamknięty musi być czasem przyszłym!", "Ups...");
               return;
            }
            CloseAtValueLabel.Content = CloseAtTimePicker.Value.Value.ToString("HH:mm:ss dd-MM-yyyy");
            _time = CloseAtTimePicker.Value.Value.Subtract(DateTime.Now);
            CloseForValueLabel.Content = _time.ToString("hh\\:mm\\:ss");
         }
         else if (CloseForTimePicker.Value != null)
         {
            TimeSpan closeForTimeSpan = new TimeSpan(0, CloseForTimePicker.Value.Value.Hour,CloseForTimePicker.Value.Value.Minute, CloseForTimePicker.Value.Value.Second);
            CloseAtValueLabel.Content = DateTime.Now.Add(closeForTimeSpan).ToString("HH:mm:ss dd-MM-yyyy");
            CloseForValueLabel.Content = _time = closeForTimeSpan;
         }
         else if (_shortcutHour > 0)
         {
            CloseAtValueLabel.Content = DateTime.Now.AddHours(_shortcutHour).ToString("HH:mm:ss dd-MM-yyyy");
            CloseForValueLabel.Content = _time = new TimeSpan(0, _shortcutHour, 0, 0);
         }
         PrepareLook();
         RunTimer();
      }

      private void OneHourShortcut_Click(object sender, RoutedEventArgs e)
      {
         _shortcutHour = 1;
         ResetTimePickers();
         TwoHourShortcut.IsChecked = false;
         ThreeHourShortcut.IsChecked = false;
      }

      private void TwoHourShortcut_Click(object sender, RoutedEventArgs e)
      {
         _shortcutHour = 2;
         ResetTimePickers();
         OneHourShortcut.IsChecked = false;
         ThreeHourShortcut.IsChecked = false;
      }

      private void ThreeHourShortcut_Click(object sender, RoutedEventArgs e)
      {
         _shortcutHour = 3;
         ResetTimePickers();
         OneHourShortcut.IsChecked = false;
         TwoHourShortcut.IsChecked = false;
      }

      private void CloseForTimePicker_LostFocus(object sender, RoutedEventArgs e)
      {
         if (CloseForTimePicker.Value != null)
            CloseAtTimePicker.Value = null;
      }

      private void CloseAtTimePicker_LostFocus(object sender, RoutedEventArgs e)
      {
         if (CloseAtTimePicker.Value != null)
            CloseForTimePicker.Value = null;
      }


      private void CloseForTimePicker_GotFocus(object sender, RoutedEventArgs e)
      {
         UncheckedToggleButtons();
      }

      private void CloseAtTimePicker_GotFocus(object sender, RoutedEventArgs e)
      {
         UncheckedToggleButtons();
      }

      private void PrepareLook()
      {
         Window.Background = new SolidColorBrush(Color.FromRgb(247, 186, 186));
         DisableAllControls();
         UncheckedToggleButtons();
         StartGroupBox.Visibility = Visibility.Hidden;
         CancelGroupBox.Visibility = Visibility.Visible;
      }

      private void DisableAllControls()
      {
         OneHourShortcut.IsEnabled = false;
         TwoHourShortcut.IsEnabled = false;
         ThreeHourShortcut.IsEnabled = false;
         CloseAtTimePicker.IsEnabled = false;
         CloseForTimePicker.IsEnabled = false;
      }

      private void EnableAllControls()
      {
         OneHourShortcut.IsEnabled = true;
         TwoHourShortcut.IsEnabled = true;
         ThreeHourShortcut.IsEnabled = true;
         CloseAtTimePicker.IsEnabled = true;
         CloseForTimePicker.IsEnabled = true;
      }

      private void UncheckedToggleButtons()
      {
         _shortcutHour = 0;
         OneHourShortcut.IsChecked = false;
         TwoHourShortcut.IsChecked = false;
         ThreeHourShortcut.IsChecked = false;
      }

      private void ResetTimePickers()
      {
         CloseForTimePicker.Value = null;
         CloseAtTimePicker.Value = null;
      }

      private void RunTimer()
      {
         _timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
         {
            if (_time == TimeSpan.Zero)
            {
               _timer.Stop();
               ShutdownSystem();
            }
            _time = _time.Add(TimeSpan.FromSeconds(-1));
            CloseForValueLabel.Content = _time.ToString("hh\\:mm\\:ss"); 
         }, Application.Current.Dispatcher);

         _timer.Start();
      }

      private void ShutdownSystem()
      {
         var psi = new ProcessStartInfo("shutdown", "/s /f /t 0")
         {
            CreateNoWindow = true,
            UseShellExecute = false
         };
         Process.Start(psi);
      }
   }
}
