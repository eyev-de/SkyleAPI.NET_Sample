using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Skyle;

namespace Skyle_Sample
{
    public partial class MainWindow : Window
    {
        private Client skyleClient = new();

        public MainWindow()
        {
            InitializeComponent();
            this.Closing += MainWindow_Closing;
            skyleClient.IsConnected += SkyleClient_IsConnected;
        }

        private void SkyleClient_IsConnected(bool obj)
        {
            Dispatcher.Invoke(() =>
            {
                Conn.Fill = new SolidColorBrush(obj ? Colors.Green : Colors.Red);
            });
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            skyleClient.Positioning -= Show_EyePosition;
            skyleClient.Gaze -= Show_Gaze;
        }

        private void Show_EyePosition(Positioning posData)
        {
            Dispatcher.Invoke(() =>
            {
                Canvas.SetLeft(leftEye, posData.LeftEye.X);
                Canvas.SetTop(leftEye, posData.LeftEye.Y);

                Canvas.SetLeft(rightEye, posData.RightEye.X);
                Canvas.SetTop(rightEye, posData.RightEye.Y);
            });
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            skyleClient.ConnectAsync();
            skyleClient.Positioning += Show_EyePosition;
            skyleClient.Gaze += Show_Gaze;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Calib_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                CalibBubble.Visibility = Visibility.Visible;
                GazeBubble.Visibility = Visibility.Hidden;
            });
            skyleClient.NextCalibPoint += a => Dispatcher.Invoke(() =>
            {
                Canvas.SetLeft(CalibBubble, a.X);
                Canvas.SetTop(CalibBubble, a.Y);
            });
            skyleClient.CalibrationFinished += () => Dispatcher.Invoke(() =>
            {
                CalibBubble.Visibility = Visibility.Hidden;
                GazeBubble.Visibility = Visibility.Visible;
            });
            skyleClient.Calibrate((int)ActualWidth, (int)ActualHeight);
        }

        private void Show_Gaze(Skyle.Point obj)
        {
            Dispatcher.Invoke(() =>
            {
                Canvas.SetLeft(GazeBubble, obj.X);
                Canvas.SetTop(GazeBubble, obj.Y);
            });
        }
    }
}
