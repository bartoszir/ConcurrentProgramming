using System;
using System.Windows;
using Billiards.Presentation.ViewModel;

namespace Billiards.PresentationView
{
    /// <summary>
    /// View implementation
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            //Random random = new Random();
            InitializeComponent();
            //MainWindowViewModel viewModel = (MainWindowViewModel)DataContext;
            //double screenWidth = SystemParameters.PrimaryScreenWidth;
            //double screenHeight = SystemParameters.PrimaryScreenHeight;
            //viewModel.Start(random.Next(5, 10));

            this.SizeChanged += (s, e) =>
            {
                if (DataContext is MainWindowViewModel vm)
                {
                    // odejmujemy marginesy i obramowanie (20px)
                    double borderThickness = 20.0;

                    // pobieramy dostępne wymiary Bordera
                    double newWidth = TableBorder.ActualWidth - borderThickness;
                    double newHeight = TableBorder.ActualHeight - borderThickness;

                    if (newWidth > 0 && newHeight > 0)
                    {
                        vm.TableWidth = newWidth;
                        vm.TableHeight = newHeight;
                    }
                }
            };
        }

        /// <summary>
        /// Raises the <seealso cref="System.Windows.Window.Closed"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected override void OnClosed(EventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel)
                viewModel.Dispose();
            base.OnClosed(e);
        }

        private void MainGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (DataContext is MainWindowViewModel vm)
            {
                vm.TableWidth = MainGrid.ActualWidth - 100.0;
                vm.TableHeight = MainGrid.ActualHeight - 120.0;
            }
        }



    }
}