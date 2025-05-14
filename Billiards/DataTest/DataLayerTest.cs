using System;
using System.Collections.Generic;
using Xunit;
using Billiards.Data;

namespace Billiards.DataTest
{
    public class BallTests
    {
        [Fact]
        public void Constructor_InitializesPositionAndVelocity()
        {
            Vector pos = new(100, 100);
            Vector vel = new(2.0, -3.0);
            double mass = 2.0;
            Ball ball = new(pos, vel, mass);

            Assert.Equal(pos.x, ((Vector)ball.Position).x);
            Assert.Equal(pos.y, ((Vector)ball.Position).y);
            Assert.Equal(vel.x, ball.Velocity.x);
            Assert.Equal(vel.y, ball.Velocity.y);
            Assert.Equal(mass, ball.Mass);
            Assert.True(ball.Diameter > 0);
        }

        [Fact]
        public void Move_ChangesPosition_And_RaisesEvent()
        {
            Vector start = new(0, 0);
            Vector vel = new(0, 0);
            double mass = 2.0;
            Ball ball = new(start, vel, mass);

            int eventCount = 0;
            ball.NewPositionNotification += (sender, newPos) =>
            {
                Assert.Equal(5, newPos.x);
                Assert.Equal(7, newPos.y);
                eventCount++;
            };

            ball.Move(new Vector(5, 7));
            Assert.Equal(1, eventCount);
        }
    }

    public class DataImplementationTests
    {
        [Fact]
        public void Start_CreatesBalls_And_CallsCallback()
        {
            var data = new DataImplementation();
            int count = 0;

            data.Start(3, (pos, ball) =>
            {
                Assert.NotNull(pos);
                Assert.NotNull(ball);
                count++;
            });

            Assert.Equal(3, count);
        }

        [Fact]
        public void Dispose_ClearsBallsList_AndDisposesTimer()
        {
            //var data = new DataImplementation();
            //data.Start(2, (_, _) => { });

            //bool disposedFlag = false;
            //data.CheckObjectDisposed(disposed => disposedFlag = disposed);

            //data.Dispose();
            //Assert.True(disposedFlag);

            //data.CheckNumberOfBalls(n => Assert.Equal(0, n));
        }

        [Fact]
        public void Move_KeepsBallsWithinBounds()
        {
            var data = new DataImplementation();
            data.Start(1, (_, _) => { });

            data.CheckBallsList(list =>
            {
                foreach (var b in list)
                {
                    Vector pos = (Vector)((Ball)b).Position;
                    Assert.InRange(pos.x, 0, 380);
                    Assert.InRange(pos.y, 0, 400);
                }
            });
        }

        public class VectorTests
        {
            [Fact]
            public void Vector_Operators_Work_Correctly()
            {
                Vector v1 = new(2, 3);
                Vector v2 = new(1, 1);

                Vector sum = v1 + v2;
                Vector diff = v1 - v2;
                Vector scaled = v1 * 2;
                Vector divided = v1 / 2;
                double dot = Vector.Dot(v1, v2);
                double length = v1.Length();

                Assert.Equal(3, sum.x);
                Assert.Equal(4, sum.y);
                Assert.Equal(1, diff.x);
                Assert.Equal(2, diff.y);
                Assert.Equal(4, scaled.x);
                Assert.Equal(6, scaled.y);
                Assert.Equal(1, divided.x);
                Assert.Equal(1.5, divided.y);
                Assert.Equal(5, dot);
                Assert.Equal(Math.Sqrt(13), length, 5);
            }

            [Fact]
            public void Vector_DivideByZero_Throws()
            {
                Vector v = new(1, 1);
                Assert.Throws<DivideByZeroException>(() => { var result = v / 0; });
            }
        }
    }
}
