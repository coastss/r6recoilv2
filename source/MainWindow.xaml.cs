using System.Reflection.Metadata;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Diagnostics;
using r6recoilv2.Utilities;
using System.Data;
using System.Runtime.InteropServices;

namespace r6recoilv2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Shared Variables
        public static string CurrentOperators = "Attackers";
        public static string SelectedOperator = "";
        public static int FireMode = 1;
        public static bool RecoilControl = false;
        public static int XAxisValue = 0;
        public static int YAxisValue = 0;
        public static bool IsSiegeAc = false;
        public static bool MouseOverProgram = false;

        // ts hittin
        public MainWindow()
        {

            InitializeComponent();
            this.Title = Helper.RandomString(16);

            UpdateStatus();
            UpdateComboBoxList();

            Thread GameToggleLoop = new Thread(new ThreadStart(ToggleLoop));
            GameToggleLoop.Start();

            Thread GameRecoilLoop = new Thread(new ThreadStart(Game.RecoilLoop));
            GameRecoilLoop.Start();
        }

        private void UpdateStatus()
        {
            string RecoilStatus = "disabled";
            //string WindowStatus = "inactive";

            if (RecoilControl)
            {
                RecoilStatus = "enabled";
            }

            /*if (IsSiegeAc)
            {
                WindowStatus = "active";
            }*/

            //BottomStatusText.Content = String.Format(SplashScreen.R6RVersion + " | siege focus: {0} | recoil: {1}", WindowStatus, RecoilStatus);
            BottomStatusText.Content = String.Format(SplashScreen.R6RVersion + " | recoil: {0}", RecoilStatus);
        }

        /////////////////////////////////
        /////////////////////////////////
        /////////////////////////////////

        // Top Bar Controls
        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }






        /////////////////////////////////
        /////////////////////////////////
        /////////////////////////////////

        // Sliders
        private void XAxisSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (XAxisSlider.Value > 0)
            {
                XAxisSlider.SelectionEnd = e.NewValue;
                XAxisSlider.SelectionStart = 0;
            }
            else
            {
                XAxisSlider.SelectionEnd = 0;
                XAxisSlider.SelectionStart = e.NewValue;
            }

            double DoMath = Convert.ToDouble(Math.Round(e.NewValue, 2));
            XAxisSliderValue.Content = DoMath;
            XAxisValue = (int)e.NewValue;
            UpdateSaveData();
        }

        private void YAxisSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (YAxisSlider.Value > 0)
            {
                YAxisSlider.SelectionEnd = e.NewValue;
                YAxisSlider.SelectionStart = 0;
            }
            else
            {
                YAxisSlider.SelectionEnd = 0;
                YAxisSlider.SelectionStart = e.NewValue;
            }

            double DoMath = Convert.ToDouble(Math.Round(e.NewValue, 2));
            YAxisSliderValue.Content = DoMath;
            YAxisValue = (int)e.NewValue;
            UpdateSaveData();
        }


        /////////////////////////////////
        /////////////////////////////////
        /////////////////////////////////
        
        private void UpdateComboBoxList()
        {
            OperatorsListBox.Items.Clear();
            var DefendersList = Operators.GetAllDataPointsForCategory(CurrentOperators);
            int _ForCount = 0;

            foreach (var Defender in DefendersList)
            {
                _ForCount += 1;
                OperatorsListBox.Items.Add(Defender.Name);

                if (_ForCount == 1)
                {
                    OperatorsListBox.SelectedItem = Defender.Name;
                }
            }
        }

        private void UpdateSaveData()
        {
            Operators.UpdateDataPoint(CurrentOperators, SelectedOperator, XAxisValue.ToString(), YAxisValue.ToString());
        }
        private void LoadSaveData()
        {
            var DataPoint = Operators.GetDataPoint(CurrentOperators, SelectedOperator);
            

            if (DataPoint != null)
            {
                XAxisSlider.Value = Math.Round(Convert.ToDouble(DataPoint.Value.X), 2);
                YAxisSlider.Value = Math.Round(Convert.ToDouble(DataPoint.Value.Y), 2);
            }
            else
            {
                MessageBox.Show("Failed to retrieve save data for: " + SelectedOperator, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }



        /////////////////////////////////
        /////////////////////////////////
        /////////////////////////////////

        // Radio Buttons
        private void AttackersRadioButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentOperators = "Attackers";
            DefendersRadioButton.IsChecked = false;
            AttackersRadioButton.IsChecked = true;

            UpdateComboBoxList();
        }

        private void DefendersRadioButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentOperators = "Defenders";
            DefendersRadioButton.IsChecked = true;
            AttackersRadioButton.IsChecked = false;

            UpdateComboBoxList();
        }

        /////////////////////////////////
        /////////////////////////////////
        /////////////////////////////////


        // Retarded CBOX
        private void OperatorsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OperatorsListBox.SelectedItem != null)
            {
                #pragma warning disable CS8601 // Possible null reference assignment.
                SelectedOperator = OperatorsListBox.SelectedValue.ToString();
                #pragma warning restore CS8601 // Possible null reference assignment.

                LoadSaveData();
            }
        }


        /////////////////////////////////
        /////////////////////////////////
        /////////////////////////////////

        private void ModeCycleButton_Click(object sender, RoutedEventArgs e)
        {
            FireMode += 1;

            if (FireMode > 2)
            {
                FireMode = 1;
            }

            if (FireMode == 1) // ADS Only
            {
                ModeCycleButton.Content = "MODE: ADS Only";
            }
            else if (FireMode == 2) // Hipfire Only
            {
                ModeCycleButton.Content = "MODE: Hipfire Only";
            }
        }

        public static SolidColorBrush PastelRed = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF4B4B"));
        public static SolidColorBrush PastelGreen = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF53EF70"));
        public static SolidColorBrush PastelBlack = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF373737"));

        private static bool RecoilControlBusy = false;
        private void ToggleRecoilControl()
        {
            if (RecoilControlBusy)
            {
                return;
            }
            RecoilControlBusy = true;
            RecoilControl = !RecoilControl;

            if (RecoilControl)
            {
                StatusButton.Foreground = PastelGreen;
            }
            else
            {
                StatusButton.Foreground = PastelRed;
            }

            UpdateStatus();
            RecoilControlBusy = false;
        }

        private void StatusButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleRecoilControl();
        }

        // Token: 0x06000012 RID: 18
        [DllImport("User32.dll")]
        public static extern short GetAsyncKeyState(Keys vKey);

        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(ushort virtualKeyCode);
        private async void ToggleLoop()
        {
            while (true)
            {
                short KeyState = GetAsyncKeyState(0x71);
                bool F2Pressed = ((KeyState >> 15) & 0x0001) == 0x0001;

                MouseOverProgram = this.IsMouseOver;

                this.Dispatcher.Invoke(() =>
                {

                    if (this.IsActive)
                    {
                        this.WindowBorder.BorderBrush = PastelRed;
                    }
                    else
                    {
                        this.WindowBorder.BorderBrush = PastelBlack;
                    }

                    if (F2Pressed)
                    {
                        ToggleRecoilControl();
                    }
                });

            await Task.Delay(100);
            }

        }
    }
} 