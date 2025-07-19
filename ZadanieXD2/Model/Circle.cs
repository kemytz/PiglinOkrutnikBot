using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ZadanieXD2.Model
{
    class Circle : INotifyPropertyChanged
    {
        private double _a;
        private double _b;
        private double _radius;

        public event PropertyChangedEventHandler? PropertyChanged;


        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public Circle(double a, double b, double radius)
        {
            _a = a;
            _b = b;
            _radius = radius * 16;
        }


        public double A
        {
            get { return _a; }
            set 
            {
                _a = value;
                OnPropertyChanged();
            }
        }


        public double B
        {
            get { return _b; }
            set
            {
                _b = value;
                OnPropertyChanged();
            }
        }


        public double Radius
        {
            get { return _radius; }
            set
            {
                _radius = value * 16;
                OnPropertyChanged();
            }
        }

        public void ResetCircle()
        {
            A = 0;
            B = 0;
            Radius = 0;
        }

        public static double[] CalculateEntityPosition(Circle measurement1, Circle eliminationMeasurement, Circle measurement2)
        {
            double radius1 = measurement1._radius;
            double radius2 = measurement2._radius;
            double distanceBetweenTwoCircles = Math.Sqrt(Math.Pow(measurement2._a - measurement1._a, 2) + Math.Pow(measurement2._b - measurement1._b, 2));

            if (distanceBetweenTwoCircles == 0)
            {
                return null;
            }

            double a = (Math.Pow(radius1, 2) - Math.Pow(radius2, 2) + Math.Pow(distanceBetweenTwoCircles, 2)) / (2 * distanceBetweenTwoCircles);

            // Check if circles are separate
            if (distanceBetweenTwoCircles > radius1 + radius2)
            {
                return null;
            }

            // Check if one circle is inside another
            if (distanceBetweenTwoCircles < Math.Abs(radius1 - radius2))
            {
                return null;
            }

            // Check if one circle is the same as the other (infinite number of solutions)
            if (distanceBetweenTwoCircles == 0 && radius1 == radius2)
            {
                return null;
            }

            // Check if elimination measurement is the same as one of the measurements
            if (eliminationMeasurement._a == measurement1._a && eliminationMeasurement._b == measurement1._b && eliminationMeasurement._radius == measurement1._radius)
            {
                return null;
            }
            if (eliminationMeasurement._a == measurement2._a && eliminationMeasurement._b == measurement2._b && eliminationMeasurement._radius == measurement2._radius)
            {
                return null;
            }

            double helpPointX = measurement1._a + a * (measurement2._a - measurement1._a) / distanceBetweenTwoCircles;
            double helpPointZ = measurement1._b + a * (measurement2._b - measurement1._b) / distanceBetweenTwoCircles;

            double hSquared = Math.Pow(radius1, 2) - Math.Pow(a, 2);
            double h;

            if (hSquared < 0)
            {
                h = -Math.Sqrt(Math.Abs(hSquared));
            }
            else
            {
                h = Math.Sqrt(hSquared);
            }

            double entityX1 = helpPointX + h * (measurement2._b - measurement1._b) / distanceBetweenTwoCircles;
            double entityZ1 = helpPointZ - h * (measurement2._a - measurement1._a) / distanceBetweenTwoCircles;

            double entityX2 = helpPointX - h * (measurement2._b - measurement1._b) / distanceBetweenTwoCircles;
            double entityZ2 = helpPointZ + h * (measurement2._a - measurement1._a) / distanceBetweenTwoCircles;

            double entity1CircleEquation = Math.Pow(entityX1 - eliminationMeasurement._a, 2) + Math.Pow(entityZ1 - eliminationMeasurement._b, 2);

            if (entity1CircleEquation > Math.Pow(eliminationMeasurement._radius, 2))
            {
                return [entityX1, entityZ1];
            }
            else
            {
                return [entityX2, entityZ2];
            }
        }
    }
}
