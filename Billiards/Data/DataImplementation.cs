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

                lastUpdateTimes[newBall] = DateTime.UtcNow;

                BallTasks.Add(Task.Run(async () =>
                {
                    while (!_cts.Token.IsCancellationRequested)
                    {
                        DateTime now = DateTime.UtcNow;
                        double deltaTime;

                        lock (_lock)
                        {
                            deltaTime = (now - lastUpdateTimes[newBall]).TotalSeconds;
                            lastUpdateTimes[newBall] = now;

                            newBall.MoveWithDelta(deltaTime);
                            HandleCollisionsForBall(newBall);
                        }

                        await Task.Delay(1, _cts.Token); // szybciej reaguje, ale deltaTime decyduje o ruchu
                    }
                }, _cts.Token));
            }
        }

        public override void SetTableSize(double width, double height)
        {
            TableWidth = width;
            TableHeight = height;

            // aktualizujemy istniejace juz kule
            lock (_lock)
            {
                foreach (Ball ball in BallsList)
                {
                    ball.UpdateTableSize(width, height);
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

        // wymiary stolu (dla dynamicznego przekazywania)
        public override double TableWidth { get; set; } = 380.0;
        public override double TableHeight { get; set; } = 400.0;

        private readonly Dictionary<Ball, DateTime> lastUpdateTimes = new();

        private Ball CreateBall()
        {
            Vector pos = new(RandomGenerator.Next(100, 300), RandomGenerator.Next(100, 300));
            //Vector vel = new((RandomGenerator.NextDouble() - 0.5) * 10, (RandomGenerator.NextDouble() - 0.5) * 10);
            double baseSpeed = 200.0; // pikseli/s 
            double angle = RandomGenerator.NextDouble() * 2 * Math.PI;
            double vx = Math.Cos(angle) * baseSpeed;
            double vy = Math.Sin(angle) * baseSpeed;
            Vector vel = new(vx, vy);
            
            //double mass = 2.0;
            double mass = RandomGenerator.NextDouble() * 0.5 + 1.0;
            //Debug.WriteLine($"U¿ywany rozmiar sto³u: width={TableWidth}, height={TableHeight}");
            return new Ball(pos, vel, mass, TableWidth, TableHeight);
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