using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZadanieXD2.Model;
using System.Collections.ObjectModel;
using ZadanieXD2.MVVM;
using System.Diagnostics;
using System.Windows;
using ZadanieXD2.Services;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Media;

namespace ZadanieXD2.ViewModel
{
    class MainWindowViewModel : ViewModelBase, IDataErrorInfo
    {
        private string _entityCoordinates;
        private Brush _coordsColor = Brushes.LightGreen;
        private string _renderDistance;
        private Circle measurement1;
        private Circle eliminationMeasurement;
        private Circle measurement2;

        private string _clipboardText;
        private readonly ClipboardService _clipboardService;
        private double[] _extractedCoords;
        public RelayCommand SetEntityTextCommand => new RelayCommand(execute => SetEntityText(""));
        public RelayCommand CloseWindowCommand => new RelayCommand(execute => CloseWindow());
        public RelayCommand MinimizeWindowCommand => new RelayCommand(execute => MinimizeWindow());
        public RelayCommand ResetCommand => new RelayCommand(execute => Reset());

        public ObservableCollection<Circle> Measurements { get; set; }

        public MainWindowViewModel(ClipboardService clipboardService)
        {
            _clipboardService = clipboardService;
            _clipboardService.ClipboardTextChanged += ExtractCoordsFromCopiedText;
            _clipboardService.ClipboardTextChanged += AddMeasurement;
            _clipboardService.ClipboardTextChanged += SetEntityText;

            Measurements = new ObservableCollection<Circle>();
            measurement1 = new Circle(0, 0, 0);
            eliminationMeasurement = new Circle(0, 0, 0);
            measurement2 = new Circle(0, 0, 0);
            Measurements.Add(measurement1);
            Measurements.Add(eliminationMeasurement);
            Measurements.Add(measurement2);
        }

        private void ExtractCoordsFromCopiedText(string text)
        {
            if (text == null)
            {
                _extractedCoords = null;
                return;
            }

            string checkIfF3C = "/execute";

            if (!text.StartsWith(checkIfF3C))
            {
                _extractedCoords = null;
                return;
            }
            double x;
            double z;

            string xText = ExtractWordSeparatedBySpaces(text, 6);

            if (!double.TryParse(xText, System.Globalization.CultureInfo.InvariantCulture, out x))
            {
                _extractedCoords = null;
                return;
            }

            string zText = ExtractWordSeparatedBySpaces(text, 8);

            if (!double.TryParse(zText, System.Globalization.CultureInfo.InvariantCulture, out z))
            {
                _extractedCoords = null;
                return;
            }

            _extractedCoords = new double[] { x, z };
        }

        private string ExtractWordSeparatedBySpaces(string fullText, int atWhichSpace)
        {
            int textLength = fullText.Length;
            int startIndex = 0;

            for (int i = 0; i < atWhichSpace; i++)
            {
                startIndex = fullText.IndexOf(" ", startIndex) + 1;
            }

            int lastIndex = fullText.IndexOf(" ", startIndex);
            int wordLength = lastIndex - startIndex;

            string word = null;

            try
            {
                word = fullText.Substring(startIndex, wordLength);
            }
            catch (Exception e)
            {
                word = null;
            }

            return word;
        }

        private void AddMeasurement(string _)
        {
            if (_extractedCoords == null)
            {
                return;
            }

            if (!int.TryParse(RenderDistance, out int rd))
            {
                return;
            }

            if (rd < 2 || rd > 32)
            {
                return;
            }

            foreach (Circle measurement in Measurements)
            {
                if (measurement.Radius == 0)
                {
                    measurement.A = _extractedCoords[0];
                    measurement.B = _extractedCoords[1];
                    measurement.Radius = rd;
                    return;
                }
            }

            measurement2.A = _extractedCoords[0];
            measurement2.B = _extractedCoords[1];
            measurement2.Radius = rd;
        }

        public string EntityCoordinates
        {
            get
            {
                return _entityCoordinates;
            }
            set
            {
                _entityCoordinates = value;
                OnPropertyChanged();
            }
        }

        public Brush CoordsColor
        {
            get
            {
                return _coordsColor;
            }
            set
            {
                _coordsColor = value;
                OnPropertyChanged();
            }
        }

        public string RenderDistance
        {
            get
            {
                return _renderDistance;
            }
            set
            {
                _renderDistance = value;
                OnPropertyChanged();
            }
        }

        public Circle SelectedMeasurement
        {
            get; set;
        }

        public string ClipboardText
        {
            get
            {
                return _clipboardText;
            }
            set
            {
                _clipboardText = value;
                OnPropertyChanged();
            }
        }

        public string Error { get { return null; } }

        public string this[string name]
        {
            get
            {
                string result = null;

                switch (name)
                {
                    case "RenderDistance":
                        if (string.IsNullOrWhiteSpace(RenderDistance))
                        {
                            result = "Render distance cannot be empty";
                        }
                        else if (!(int.TryParse(RenderDistance, out int i)) || i < 2 || i > 32)
                        {
                            result = "Inproper renderdistance set";
                        }
                        break;
                }

                return result;
            }
        }

        private void CloseWindow()
        {
            Application.Current.Shutdown();
        }

        private void MinimizeWindow()
        {
            var window = Application.Current.MainWindow;
            if (window != null)
            {
                window.WindowState = WindowState.Minimized;
            }
        }

        private void SetEntityText(string _)
        {
            if (measurement1.Radius == 0 || eliminationMeasurement.Radius == 0 || measurement2.Radius == 0)
            {
                return;
            }

            double[] entityPosition = Circle.CalculateEntityPosition(measurement1, eliminationMeasurement, measurement2);

            if (entityPosition == null)
            {
                CoordsColor = Brushes.Red;
                EntityCoordinates = "Could not find an Entity due to incorrect measurements!";
                return;
            }

            double roundedX = double.Round(entityPosition[0], 3);
            double roundedZ = double.Round(entityPosition[1], 3);

            if (entityPosition != null)
            {
                CoordsColor = Brushes.LightGreen;
                EntityCoordinates = "Successfully found an Entity at X: " + roundedX.ToString(CultureInfo.InvariantCulture) + " Z: " + roundedZ.ToString(CultureInfo.InvariantCulture);
            }
        }

        private void Reset()
        {
            RenderDistance = null;
            foreach (Circle measurement in Measurements)
            {
                measurement.ResetCircle();
            }
            EntityCoordinates = null;
            _extractedCoords = null;
        }
    }
}
