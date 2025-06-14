using System;
using System.ComponentModel;

namespace Billiards.Presentation.Model
{
    public interface IBall : INotifyPropertyChanged
    {
        double Top { get; }
        double Left { get; }
        double Diameter { get; }
    }

    public abstract class ModelAbstractApi : IObservable<IBall>, IDisposable
    {
        public static ModelAbstractApi CreateModel()
        {
            return modelInstance.Value;
        }

        public abstract void Start(int numberOfBalls, double tableWidth, double tableHeight);

        public abstract void SetTableSize(double width, double height);

        #region IObservable

        public abstract IDisposable Subscribe(IObserver<IBall> observer);

        #endregion IObservable

        #region IDisposable

        public abstract void Dispose();

        #endregion IDisposable

        #region private

        private static Lazy<ModelAbstractApi> modelInstance = new Lazy<ModelAbstractApi>(() => new ModelImplementation());

        #endregion private
    }
}