using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MediaPlayerApp
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer _timer;

        public MainWindow()
        {
            InitializeComponent();
            MediaPlayer.Volume = 0.5; // Устанавливаем громкость по умолчанию 50%

            // Таймер для обновления положения слайдера
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += UpdatePositionSlider;
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MediaPlayer.Source = new Uri(FilePathTextBox.Text, UriKind.Absolute);
                MediaPlayer.LoadedBehavior = MediaState.Manual;
                MediaPlayer.UnloadedBehavior = MediaState.Manual;
                MediaPlayer.Stop();
                PlayPauseButton.Content = "Play";
                PositionSlider.Value = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (MediaPlayer.Source == null)
            {
                MessageBox.Show("Please load a video file first.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MediaPlayer.CanPause && MediaPlayer.Position != TimeSpan.Zero)
            {
                MediaPlayer.Pause();
                PlayPauseButton.Content = "Play";
                _timer.Stop();
            }
            else
            {
                MediaPlayer.Play();
                PlayPauseButton.Content = "Pause";
                _timer.Start();
            }
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MediaPlayer.Volume = VolumeSlider.Value / 100;
        }

        private void UpdatePositionSlider(object sender, EventArgs e)
        {
            if (MediaPlayer.NaturalDuration.HasTimeSpan)
            {
                PositionSlider.Maximum = MediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                PositionSlider.Value = MediaPlayer.Position.TotalSeconds;
            }
        }

        private void PositionSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (MediaPlayer.NaturalDuration.HasTimeSpan && PositionSlider.IsMouseOver)
            {
                MediaPlayer.Position = TimeSpan.FromSeconds(PositionSlider.Value);
            }
        }

        private void MediaPlayer_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MessageBox.Show("Failed to load video. Please check the file path.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
