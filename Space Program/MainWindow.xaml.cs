using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace HoloAssist
{
    public partial class MainWindow : Window
    {
        private readonly OfflineLanguageModel offlineLanguageModel;

        public MainWindow()
        {
            InitializeComponent();
            offlineLanguageModel = new OfflineLanguageModel();
            SimulateWeatherUpdate();
            SimulateNewsUpdate();
        }

        private async void CommandInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string command = CommandInput.Text;
                CommandInput.Clear(); // Clear the input immediately

                Console.WriteLine($"Enter key pressed. Command: {command}");

                try
                {
                    await ProcessCommandAsync(command);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in CommandInput_KeyDown: {ex.Message}");
                    DisplayAssistantResponse($"Error: {ex.Message}");
                }
            }
        }

        private async Task ProcessCommandAsync(string command)
        {
            try
            {
                Console.WriteLine($"Processing command: {command}");
                DisplayAssistantResponse($"Processing command: {command}");

                Console.WriteLine("Sending request to language model...");
                DisplayAssistantResponse("Sending request to language model...");

                string response = await offlineLanguageModel.GenerateResponseAsync(command);

                Console.WriteLine($"Response received from GenerateResponseAsync: {response}");
                DisplayAssistantResponse($"AI: {response}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ProcessCommandAsync: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                DisplayAssistantResponse($"Error processing command: {ex.Message}");
            }
        }

        private void DisplayAssistantResponse(string response)
        {
            Console.WriteLine($"Attempting to display response: {response}");
            Dispatcher.Invoke(() =>
            {
                TextBlock responseBlock = new TextBlock
                {
                    Text = response,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 0, 0, 10)
                };

                TaskList.Items.Insert(0, responseBlock);
                Console.WriteLine("Response added to UI");
            });
        }

        private void RemoveTask(string taskToRemove)
        {
            /*int index = tasks.FindIndex(t => t.Equals(taskToRemove, StringComparison.OrdinalIgnoreCase));
            if (index != -1)
            {
                tasks.RemoveAt(index);
                AnimateTaskRemoval(index);
            }
            else
            {
                MessageBox.Show($"Task not found: {taskToRemove}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }*/
        }

        private void AnimateTaskRemoval(int index)
        {
            ListBoxItem item = (ListBoxItem)TaskList.ItemContainerGenerator.ContainerFromIndex(index);
            if (item != null)
            {
                DoubleAnimation opacityAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
                DoubleAnimation translateAnimation = new DoubleAnimation(0, 50, TimeSpan.FromSeconds(0.5));
                translateAnimation.Completed += (s, e) => UpdateTaskList();

                item.BeginAnimation(UIElement.OpacityProperty, opacityAnimation);
                ((TranslateTransform)item.RenderTransform).BeginAnimation(TranslateTransform.XProperty, translateAnimation);
            }
            else
            {
                UpdateTaskList();
            }
        }

        private void UpdateTaskList()
        {
            /*TaskList.Items.Clear();
            foreach (string task in tasks)
            {
                TaskList.Items.Add(task);
            }*/
        }

        private void SimulateWeatherUpdate()
        {
            WeatherInfo.Text = "Sunny, 72°F (22°C)\nHumidity: 45%";
        }

        private void SimulateNewsUpdate()
        {
            NewsInfo.Text = "Scientists make breakthrough in quantum computing\n\n" +
                            "New eco-friendly public transportation unveiled in major cities";
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }

        private void MainWindow_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point clickPosition = e.GetPosition(this);
            CircularMenu.Visibility = Visibility.Visible;
            Canvas.SetLeft(CircularMenu, clickPosition.X - 100);
            Canvas.SetTop(CircularMenu, clickPosition.Y - 100);
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
            CircularMenu.Visibility = Visibility.Collapsed;
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("HoloAssist v1.0\nYour futuristic personal assistant!", "About", MessageBoxButton.OK, MessageBoxImage.Information);
            CircularMenu.Visibility = Visibility.Collapsed;
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Settings functionality not implemented yet.", "Settings", MessageBoxButton.OK, MessageBoxImage.Information);
            CircularMenu.Visibility = Visibility.Collapsed;
        }

    }
}
