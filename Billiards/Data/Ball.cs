namespace Billiards.Data
{
    internal class Ball : IBall
    {
        #region ctor

        internal Vector Position { get; private set; }

        internal Ball(Vector initialPosition, Vector initialVelocity, double mass, double tableWidth, double tableHeight)
        {
            Position = initialPosition;
            Velocity = initialVelocity;
            Mass = mass;
            Diameter = CalculateDiameter(mass);
            //Diameter = 25.0;
            TableWidth = tableWidth;
            TableHeight = tableHeight;
            Id = Interlocked.Increment(ref _nextId);
        }


        #endregion ctor

        #region IBall

        public event EventHandler<IVector>? NewPositionNotification;

        public IVector Velocity { get; set; }

        public double Mass { get; private set; }

        public double Diameter { get; private set; }

        // wymiary stolu
        public double TableWidth { get; internal set; }

        public double TableHeight { get; internal set; }

        public DiagnosticLogger? Logger { get; set; }

        public int Id { get; }

        #endregion IBall

        #region private

        private static int _nextId = 1;


        private void RaiseNewPositionChangeNotification()
        {
            NewPositionNotification?.Invoke(this, Position);
        }

        internal void MoveWithDelta(double deltaTime)
        {
            double dx = Velocity.x * deltaTime;
            double dy = Velocity.y * deltaTime;

            double newX = Position.x + dx;
            double newY = Position.y + dy;

            double radius = Diameter / 2.0;

            //if (newX < 0 || newX > TableWidth - Diameter || newY < 0 || newY > TableHeight - Diameter)
            //{
            //    Logger?.Log(new DiagnosticEvent
            //    {
            //        Timestamp = DateTime.Now,
            //        EventType = "WallHit",
            //        BallId1 = this.GetHashCode(),
            //        BallId2 = null,
            //        Position = $"x={newX},y={newY}" 
            //    });
            

            if (newX < 0)
            {
                Logger?.Log(new DiagnosticEvent
                {
                    Timestamp = DateTime.Now,
                    EventType = "WallHit (Left)",
                    BallId1 = this.Id,
                    BallId2 = null,
                    Position = $"x={Math.Round(newX, 2)} ,y={Math.Round(newY, 2)}"
                });

                newX = radius;
                dx = -dx;
            }
            else if (newX > TableWidth - Diameter)
            {
                Logger?.Log(new DiagnosticEvent
                {
                    Timestamp = DateTime.Now,
                    EventType = "WallHit (Right)",
                    BallId1 = this.Id,
                    BallId2 = null,
                    Position = $"x={Math.Round(newX, 2)},y={Math.Round(newY, 2)}"
                });

                newX = TableWidth - Diameter;
                dx = -dx;
            }

            if (newY < 0)
            {
                Logger?.Log(new DiagnosticEvent
                {
                    Timestamp = DateTime.Now,
                    EventType = "WallHit (Top)",
                    BallId1 = this.Id,
                    BallId2 = null,
                    Position = $"x={Math.Round(newX, 2)},y={Math.Round(newY, 2)}"
                });

                newY = radius;
                dy = -dy;
            }
            else if (newY > TableHeight - Diameter)
            {
                Logger?.Log(new DiagnosticEvent
                {
                    Timestamp = DateTime.Now,
                    EventType = "WallHit (Bottom)",
                    BallId1 = this.Id,
                    BallId2 = null,
                    Position = $"x={Math.Round(newX, 2)} ,y={Math.Round(newY, 2)}"
                });

                newY = TableHeight - Diameter;
                dy = -dy;
            }

            Velocity = new Vector(dx / deltaTime, dy / deltaTime);
            Position = new Vector(newX, newY);

            RaiseNewPositionChangeNotification();
        }

        internal void UpdateTableSize(double width, double height)
        {
            TableWidth = width;
            TableHeight = height;
        }

        private static double CalculateDiameter(double mass)
        {
            const double baseDiameter = 20.0;
            const double scalingFactor = 20.0;
            return baseDiameter + (mass - 1) * scalingFactor;
        }

        #endregion private
    }
}