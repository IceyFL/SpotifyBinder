using System.Drawing;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using SpotifyBinder.Functions;
using SpotifyBinder.UserController;

namespace SpotifyBinder
{
    public partial class MainWindow : Window
    {
        public List<string> KeyHeld = new List<string> { };

        int LastRan = 10;

        private NotifyIcon notifyIcon;

        InputBindingManager KeyListener;

        AKeyChanger SkipKeyChanger;
        AKeyChanger PreviousKeyChanger;
        AKeyChanger PlayPauseKeyChanger;

        bool WinKey = false;
        bool CtrlKey = true;
        bool AltKey = true;
        bool ShftKey = true;

        bool SkipKeyChange = false;
        bool PreviousKeyChange = false;
        bool PlayPauseKeyChange = false;
        
        string SkipKey = "Right";
        string PreviousKey = "Left";
        string PlayPauseKey = "Up";

        public MainWindow()
        {
            InitializeComponent();
            InitializeNotifyIcon();
            try { LoadSettings(); } catch { }
            LoadUIElements();

            Change_Keysize(SkipKeyChanger, SkipKey);
            Change_Keysize(PreviousKeyChanger, PreviousKey);
            Change_Keysize(PlayPauseKeyChanger, PlayPauseKey);

            #region KeyListener
            KeyListener = new InputBindingManager();
            KeyListener.Setup();
            KeyListener.OnBindingPressed += (binding) =>
            {
                if (SkipKeyChange) { SkipKey = binding; SkipKeyChange = false; SkipKeyChanger.KeyNotifier.Content = binding; Change_Keysize(SkipKeyChanger, binding); }
                else if (PreviousKeyChange) { PreviousKey = binding; PreviousKeyChange = false; PreviousKeyChanger.KeyNotifier.Content = binding; Change_Keysize(PreviousKeyChanger, binding); }
                else if (PlayPauseKeyChange) { PlayPauseKey = binding; PlayPauseKeyChange = false; PlayPauseKeyChanger.KeyNotifier.Content = binding; Change_Keysize(PlayPauseKeyChanger, binding); }
                if (!KeyHeld.Contains(binding))
                {
                    KeyHeld.Add(binding);
                }
            };
            KeyListener.OnBindingReleased += (binding) => { try{ KeyHeld.Remove(binding);} catch { } };
            #endregion


            Task.Run(() => MainLoop());
        }

        private void MainLoop()
        {
            while (true)
            {
                if (CheckKeys(PlayPauseKey) && LastRan >= 10)
                {
                    MediaFunctions.PlayPause();
                    LastRan = 0;
                }
                if (CheckKeys(SkipKey) && LastRan >= 10)
                {
                    MediaFunctions.Skip();
                    LastRan = 0;
                }
                if (CheckKeys(PreviousKey) && LastRan >= 10)
                {
                    MediaFunctions.Previous();
                    LastRan = 0;
                }
                Thread.Sleep(20);
                LastRan++;
            }
        }

        private bool CheckKeys(string MainKey) 
        {
            bool AllHeld = true;
            if (WinKey && !(KeyHeld.Contains("LWin") || KeyHeld.Contains("RWin")))  {AllHeld = false;}
            if (CtrlKey && !(KeyHeld.Contains("LControlKey") || KeyHeld.Contains("RControlKey"))) { AllHeld = false;}
            if (AltKey && !(KeyHeld.Contains("LMenu") || KeyHeld.Contains("RMenu"))) { AllHeld = false; }
            if (ShftKey && !(KeyHeld.Contains("LShiftKey") || KeyHeld.Contains("RShiftKey"))) { AllHeld = false; }
            if (!KeyHeld.Contains(MainKey)) { AllHeld = false; }
            return AllHeld;
        }


        private void LoadUIElements()
        {
            ItemScroller.Children.Clear();

            AToggle WindowsKey = new("Windows Key");
            WindowsKey.ToggleCheckBox.Name = "WindowsKey";
            WindowsKey.ToggleCheckBox.Click += (s, x) =>
            {
                WinKey=!WinKey;
            };
            ItemScroller.Children.Add(WindowsKey);
            if (WinKey == true)
            {
                WindowsKey.ToggleCheckBox.IsChecked = true;
            }

            AToggle ControlKey = new("Control Key");
            ControlKey.ToggleCheckBox.Name = "ControlKey";
            ControlKey.ToggleCheckBox.Click += (s, x) =>
            {
                CtrlKey = !CtrlKey;
            };
            ItemScroller.Children.Add(ControlKey);
            if (CtrlKey == true)
            {
                ControlKey.ToggleCheckBox.IsChecked = true;
            }

            AToggle AltKeyToggle = new("Alt Key");
            AltKeyToggle.ToggleCheckBox.Name = "AltKeyToggle";
            AltKeyToggle.ToggleCheckBox.Click += (s, x) =>
            {
                AltKey = !AltKey;
            };
            ItemScroller.Children.Add(AltKeyToggle);
            if (AltKey == true)
            {
                AltKeyToggle.ToggleCheckBox.IsChecked = true;
            }

            AToggle ShftKeyToggle = new("Shift Key");
            ShftKeyToggle.ToggleCheckBox.Name = "ShftKeyToggle";
            ShftKeyToggle.ToggleCheckBox.Click += (s, x) =>
            {
                ShftKey = !ShftKey;
            };
            ItemScroller.Children.Add(ShftKeyToggle);
            if (ShftKey == true)
            {
                ShftKeyToggle.ToggleCheckBox.IsChecked = true;
            }

            SkipKeyChanger = new("Skip Key", SkipKey);
            SkipKeyChanger.Reader.Click += (s, x) =>
            {
                SkipKeyChanger.KeyNotifier.Content = "Listening..";
                Change_Keysize(SkipKeyChanger, "Listening..");
                SkipKeyChange = true;
            };
            ItemScroller2.Children.Add(SkipKeyChanger);

            PreviousKeyChanger = new("Previous Key", PreviousKey);
            PreviousKeyChanger.Reader.Click += (s, x) =>
            {
                PreviousKeyChanger.KeyNotifier.Content = "Listening..";
                Change_Keysize(PreviousKeyChanger, "Listening..");
                PreviousKeyChange = true;
            };
            ItemScroller2.Children.Add(PreviousKeyChanger);

            PlayPauseKeyChanger = new("Play/Pause Key", PlayPauseKey);
            PlayPauseKeyChanger.Reader.Click += (s, x) =>
            {
                PlayPauseKeyChanger.KeyNotifier.Content = "Listening..";
                Change_Keysize(PlayPauseKeyChanger, "Listening..");
                PlayPauseKeyChange = true;
            };
            ItemScroller2.Children.Add(PlayPauseKeyChanger);
        }


        #region UI
        private void Change_Keysize(AKeyChanger Current, string Key)
        {
            int Width = 20;
            int bindingwidth = Key.Length * 6;
            Width = Width + bindingwidth;
            Current.Box.Width = Width;
        }

        private void MainBorder_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            System.Windows.Point mousePosition = e.GetPosition(MainBorder);
            double xPercentage = mousePosition.X / MainBorder.ActualWidth;
            double yPercentage = mousePosition.Y / MainBorder.ActualHeight;

            // Calculate the angle of the gradient based on the mouse position
            double angle = Math.Atan2(yPercentage - 0.5, xPercentage - 0.5) * (180 / Math.PI);

            GradientRotation.Angle = angle;
        }
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private Icon LoadIconFromResource(string iconName)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(iconName))
            {
                if (stream != null)
                {
                    return new Icon(stream);
                }
            }
            return null;
        }

        private void InitializeNotifyIcon()
        {
            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = LoadIconFromResource("SpotifyBinder.SpotifyBinderIcon.ico");
            notifyIcon.Text = "SpotifyBinder";
            notifyIcon.Visible = true;
            notifyIcon.Click += NotifyIcon_Click;
        }

        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = WindowState.Normal;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            notifyIcon.Dispose();
            System.Windows.Application.Current.Shutdown();
            KeyListener.StopListening();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
            this.Hide(); // Hide the window when minimized
        }
        #endregion

        #region Save/Load

        public void SaveSettings()
        {
            string[] settings = new string[7];
            settings[0] = SkipKey;
            settings[1] = PreviousKey;
            settings[2] = PlayPauseKey;
            settings[3] = WinKey.ToString();
            settings[4] = CtrlKey.ToString();
            settings[5] = AltKey.ToString();
            settings[6] = ShftKey.ToString();
            System.IO.File.WriteAllLines("settings.txt", settings);
        }

        public void LoadSettings()
        {
            string[] settings = System.IO.File.ReadAllLines("settings.txt");
            SkipKey = settings[0];
            PreviousKey = settings[1];
            PlayPauseKey = settings[2];
            WinKey = Convert.ToBoolean(settings[3]);
            CtrlKey = Convert.ToBoolean(settings[4]);
            AltKey = Convert.ToBoolean(settings[5]);
            ShftKey = Convert.ToBoolean(settings[6]);
        }

        #endregion

    }
}