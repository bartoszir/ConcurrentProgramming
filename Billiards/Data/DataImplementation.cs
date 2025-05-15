using System;
using System.Diagnostics;

namespace Billiards.Data
{
    internal class DataImplementation : DataAbstractAPI
    {
        #region ctor

        public DataImplementation()
        {
            //MoveTimer = new Timer(Move, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(16.67)); // 1000 ms / 60 = 16.67 || 144hz -> 6.94
        }

        #endregion ctor

        #region DataAbstractAPI

        public override void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataImplementation));
            if (upperLayerHandler == null)
                throw new ArgumentNullException(nameof(upperLayerHandler));

            //Random random = new Random();

            lock (_lock)
            {
                BallsList.Clear();
            }

            _cts = new CancellationTokenSource();

            for (int i = 0; i < numberOfBalls; i++)
            {
                Ball newBall = CreateBall();
                upperLayerHandler(newBall.Position, newBall);

                lock (_lock)
                {
                    BallsList.Add(newBall);
                }

                BallTasks.Add(Task.Run(async () =>
                {
                    while (!_cts.Token.IsCancellationRequested)
                    {
                        lock (_lock)
                        {
                            newBall.Move(new Vector(newBall.Velocity.x, newBall.Velocity.y));
                            HandleCollisionsForBall(newBall);
                        }

                        await Task.Delay(16, _cts.Token);
                    }
                }, _cts.Token));
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
                    //MoveTimer.Dispose();
                    _cts?.Cancel();
                    _cts?.Dispose();
                    BallTasks.Clear();
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

        //private readonly Timer MoveTimer;
        private Random RandomGenerator = new();
        private List<Ball> BallsList = [];
        private readonly object _lock = new();
        private readonly List<Task> BallTasks = new();
        private CancellationTokenSource _cts = new();

        // Maksymalna predkosc
        private const double InitialSpeed = 10.0;

        private Ball CreateBall()
        {
            Vector pos = new(RandomGenerator.Next(100, 300), RandomGenerator.Next(100, 300));
            Vector vel = new((RandomGenerator.NextDouble() - 0.5) * 10, (RandomGenerator.NextDouble() - 0.5) * 10);
            //double mass = 2.0;
            double mass = RandomGenerator.NextDouble() * 0.5 + 1.0;
            return new Ball(pos, vel, mass);
        }

        private void HandleCollisionsForBall(Ball current)
        {
            foreach (Ball other in BallsList)
            {
                if (!ReferenceEquals(current, other))
                {
                    HandleCollision(current, other);
                }
            }
        }
        

        private void HandleCollision(Ball a, Ball b)
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