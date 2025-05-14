using System;
using System.Diagnostics;

namespace Billiards.Data
{
    internal class DataImplementation : DataAbstractAPI
    {
        #region ctor

        public DataImplementation()
        {
            MoveTimer = new Timer(Move, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(16.67)); // 1000 ms / 60 = 16.67 || 144hz -> 6.94
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

            lock (_lock)
            {
                BallsList.Clear();
            }

            for (int i = 0; i < numberOfBalls; i++)
            {
                // losuje pozycje srokdka z uwzglednieniem promienia
                //double x = random.NextDouble() * (TableWidth - BallDiameter) + BallRadius;
                //double y = random.NextDouble() * (TableHeight - BallDiameter) + BallRadius;
                //Vector startPos = new(x, y);
                Vector startPos = new(random.Next(100, 400 - 100), random.Next(100, 400 - 100));
                Vector startVel = new((random.NextDouble() - 0.5) * 10, (random.NextDouble() - 0.5) * 10);

                //// losuje poczatkowa predkosc 
                //double vx = (random.NextDouble() - 0.5) * InitialSpeed;
                //double vy = (random.NextDouble() - 0.5) * InitialSpeed;

                //double randomMass = random.NextDouble() * 0.5 + 1.0; // masa z zakresu 1.0 - 1.5

                double randomMass = 2.0;

                Ball ball = new(startPos, startVel, randomMass);
                upperLayerHandler(startPos, ball);

                lock (_lock)
                {
                    BallsList.Add(ball);
                }
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
        private readonly object _lock = new();

        // Maksymalna predkosc
        private const double InitialSpeed = 10.0;

        private void Move(object? x)
        {
            lock (_lock)
            {
                int count = BallsList.Count;
                for (int i = 0; i < count - 1; i++)
                {
                    for (int j = i + 1; j < count; j++)
                    {
                        HandleCollision(BallsList[i], BallsList[j]);
                    }
                }

                foreach (Ball item in BallsList)
                {
                    item.Move(new Vector(item.Velocity.x, item.Velocity.y));
                }
            }

        }

        protected void HandleCollision(Ball a, Ball b)
        {
            Vector posA = a.Position;
            Vector posB = b.Position;
            Vector velA = (Vector)a.Velocity;
            Vector velB = (Vector)b.Velocity;

            double massA = a.Mass;
            double massB = b.Mass;

            Vector offset = posA - posB;
            double actualDistance = offset.Length();
            double contactThreshold = (a.Diameter + b.Diameter) * 0.5;

            if (actualDistance >= contactThreshold || actualDistance == 0)
                return;

            Vector normal = offset / actualDistance;
            Vector relativeVelocity = velA - velB;

            double approachSpeed = Vector.Dot(relativeVelocity, normal);

            if (approachSpeed >= 0)
                return;

            double impulseMagnitude = (2.0 * approachSpeed) / (massA + massB);
            Vector impulse = impulseMagnitude * normal;

            a.Velocity = velA - impulse * massB;
            b.Velocity = velB + impulse * massA;
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