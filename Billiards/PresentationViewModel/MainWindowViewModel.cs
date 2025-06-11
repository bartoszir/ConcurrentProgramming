using System;
using System.Collections.ObjectModel;
using Billiards.Presentation.Model;
using Billiards.Presentation.ViewModel.MVVMLight;
using System.Windows.Input;
using ModelIBall = Billiards.Presentation.Model.IBall;

namespace Billiards.Presentation.ViewModel
{
    public class MainWindowViewModel : ViewModelBase, IDisposable
    {
        #region ctor

        public MainWindowViewModel() : this(null)
        { }

        internal MainWindowViewModel(ModelAbstractApi modelLayerAPI)
        {
            ModelLayer = modelLayerAPI == null ? ModelAbstractApi.CreateModel() : modelLayerAPI;
            Observer = ModelLayer.Subscribe<ModelIBall>(x => Balls.Add(x));

            _startCommand = new RelayCommand(
                execute: () => Start(NumberOfBalls),
                canExecute: () => NumberOfBalls > 0 && _isEnabled);
        }

        #endregion ctor

        #region public API

        public int NumberOfBalls
        {
            get => _numberOfBalls;
            set
            {
                _numberOfBalls = value;
                RaisePropertyChanged();
                _startCommand.RaiseCanExecuteChanged();
            }
        }

        public double TableWidth
        {
            get => _tableWidth;
            set
            {
                if (_tableWidth != value)
                {
                    _tableWidth = value;
                    RaisePropertyChanged();
                    _startCommand.RaiseCanExecuteChanged();
                    ModelLayer.SetTableSize(_tableWidth, _tableHeight);
                }
            }
        }

        public double TableHeight
        {
            get => _tableHeight;
            set
            {
                if (_tableHeight != value)
                {
                    _tableHeight = value;
                    RaisePropertyChanged();
                    _startCommand.RaiseCanExecuteChanged();
                    ModelLayer.SetTableSize(_tableWidth, _tableHeight);
                }
            }
        }

        public ICommand StartCommand => _startCommand;

        public void Start(int numberOfBalls)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(MainWindowViewModel));
            Balls.Clear();
            ModelLayer.Start(numberOfBalls, TableWidth, TableHeight);
            Observer?.Dispose();
            Observer = ModelLayer.Subscribe<ModelIBall>(x => Balls.Add(x));
        }

        public ObservableCollection<ModelIBall> Balls { get; } = new ObservableCollection<ModelIBall>();


        #endregion public API

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    _isEnabled = false;
                    _startCommand.RaiseCanExecuteChanged();
                    Observer.Dispose();
                    ModelLayer.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                Disposed = true;
            }
        }

        public void Dispose()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(MainWindowViewModel));
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable

        #region private

        private IDisposable Observer = null;
        private ModelAbstractApi ModelLayer;
        private bool Disposed = false;
        private int _numberOfBalls;
        private bool _isEnabled = true;
        private readonly RelayCommand _startCommand;
        // rozmiary stolu
        private double _tableWidth = 380.0;
        private double _tableHeight = 400.0;

        #endregion private
    }
}