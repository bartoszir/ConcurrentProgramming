using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Billiards.BusinessLogic;
using LogicIBall = Billiards.BusinessLogic.IBall;

namespace Billiards.Presentation.Model
{
    internal class ModelBall : IBall
    {
        public ModelBall(double top, double left, LogicIBall underneathBall)
        {
            TopBackingField = top;
            LeftBackingField = left;
            underneathBall.NewPositionNotification += NewPositionNotification;
            Diameter = underneathBall.Diameter;
        }

        #region IBall

        public double Top
        {
            get { return TopBackingField; }
            private set
            {
                if (TopBackingField == value)
                    return;
                TopBackingField = value;
                RaisePropertyChanged();
            }
        }

        public double Left
        {
            get { return LeftBackingField; }
            private set
            {
                if (LeftBackingField == value)
                    return;
                LeftBackingField = value;
                RaisePropertyChanged();
            }
        }

        public double Diameter { get; private set; }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion INotifyPropertyChanged

        #endregion IBall

        #region private

        private double TopBackingField;
        private double LeftBackingField;

        private void NewPositionNotification(object sender, IPosition e)
        {
            Top = e.y; Left = e.x;
        }

        private void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion private

        #region testing instrumentation

        [Conditional("DEBUG")]
        internal void SetLeft(double x)
        { Left = x; }

        [Conditional("DEBUG")]
        internal void SettTop(double x)
        { Top = x; }

        #endregion testing instrumentation
    }
}