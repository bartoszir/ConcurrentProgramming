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

        #endregion IBall

        #region private


        private void RaiseNewPositionChangeNotification()
        {
            NewPositionNotification?.Invoke(this, Position);
        }

        internal void Move(Vector delta)
        {
            double dx = delta.x;
            double dy = delta.y;

            // nowa pozycja
            double newX = Position.x + dx;
            double newY = Position.y + dy;

            double radius = Diameter / 2.0;

            // odbicie od LEWEJ krawedzi (srodek >= BallRadius)
            if (newX < 0)
            {
                newX = radius;
                dx = -dx;
            }
            // odbicie od PRAWEJ krawedzi (srodek <= TableWidth - BallRadius)
            else if (newX > TableWidth - Diameter)
            {
                newX = TableWidth - Diameter;
                dx = -dx;
            }

            // odbicie od GORNEJ krawedzi (srodek >= BallRadius)
            if (newY < 0)
            {
                newY = radius;
                dy = -dy;
            }
            // odbicie od DOLNEJ krawedzi (srodek <= TableHeight - BallRadius)
            else if (newY > TableHeight - Diameter)
            {
                newY = TableHeight - Diameter;
                dy = -dy;
            }

            Velocity = new Vector(dx, dy);
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