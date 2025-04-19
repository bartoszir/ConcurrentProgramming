using System;
using System.Diagnostics;

namespace Billiards.Data
{
    internal class DataImplementation : DataAbstractAPI
    {
        #region ctor

        public DataImplementation()
        {
            MoveTimer = new Timer(Move, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(100));
        }

        #endregion ctor

        #region DataAbstractAPI

        public override void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataImplementation));
            if (upperLayerHandler == null)
                throw new ArgumentNullException(nameof(upperLayerHandler));
            Random random = new Random();
            for (int i = 0; i < numberOfBalls; i++)
            {
                Vector startingPosition = new(random.Next(100, 400 - 100), random.Next(100, 400 - 100));
                Ball newBall = new(startingPosition, startingPosition);
                upperLayerHandler(startingPosition, newBall);
                BallsList.Add(newBall);
            }
        }

        #endregion DataAbstractAPI

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    MoveTimer.Dispose();
                    BallsList.Clear();
                }
                Disposed = true;
            }
            else
                throw new ObjectDisposedException(nameof(DataImplementation));
        }

        public override void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable

        #region private

        //private bool disposedValue;
        private bool Disposed = false;

        private readonly Timer MoveTimer;
        private Random RandomGenerator = new();
        private List<Ball> BallsList = [];

        // wymiary stolu 
        // TODO: zmienic ze sztywnego ustawiania rozmiarow na dynamiczne przekazywanie (!)
        private const double TableWidth = 380.0;
        private const double TableHeight = 400.0;

        // Rozmiar kul (œrednica i promieñ)
        private const double BallDiameter = 20.0;
        private const double BallRadius = BallDiameter / 2.0;

        private void Move(object? x)
        {
            foreach (Ball item in BallsList)
            {
                // biezaca pozycja
                var current = item.Position;

                // losowy przyrost
                double dx = (RandomGenerator.NextDouble() - 0.5) * 40;
                double dy = (RandomGenerator.NextDouble() - 0.5) * 40;

                // nowa pozycja
                double newX = current.x + dx;
                double newY = current.y + dy;


                // odbicie od LEWEJ krawedzi (srodek >= BallRadius)
                if (newX < 0)
                {
                    newX = BallRadius;
                    dx = -dx;
                }
                // odbicie od PRAWEJ krawedzi (srodek <= TableWidth - BallRadius)
                else if (newX > TableWidth - BallDiameter)
                {
                    newX = TableWidth - BallDiameter;
                    dx = -dx;
                }

                // odbicie od GORNEJ krawedzi (srodek >= BallRadius)
                if (newY < 0)
                {
                    newY = BallRadius;
                    dy = -dy;
                }
                // odbicie od DOLNEJ krawedzi (srodek <= TableHeight - BallRadius)
                else if (newY > TableHeight - BallDiameter)
                {
                    newY = TableHeight - BallDiameter;
                    dy = -dy;
                }

                // Rzeczywisty wektor przesuniêcia
                double actualDx = newX - current.x;
                double actualDy = newY - current.y;

                item.Move(new Vector(actualDx, actualDy));

                //item.Move(new Vector((RandomGenerator.NextDouble() - 0.5) * 14, (RandomGenerator.NextDouble() - 0.5) * 14));
            }

        }

        #endregion private

        #region TestingInfrastructure

        [Conditional("DEBUG")]
        internal void CheckBallsList(Action<IEnumerable<IBall>> returnBallsList)
        {
            returnBallsList(BallsList);
        }

        [Conditional("DEBUG")]
        internal void CheckNumberOfBalls(Action<int> returnNumberOfBalls)
        {
            returnNumberOfBalls(BallsList.Count);
        }

        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
        {
            returnInstanceDisposed(Disposed);
        }

        #endregion TestingInfrastructure
    }
}