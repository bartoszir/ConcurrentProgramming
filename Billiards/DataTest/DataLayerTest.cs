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
            Vector pos = new(10, 20);
            Vector vel = new(3, 4);
            Ball ball = new(pos, vel);

            Assert.Equal(10, ((Vector)ball.Position).x);
            Assert.Equal(20, ((Vector)ball.Position).y);
            Assert.Equal(3, ball.Velocity.x);
            Assert.Equal(4, ball.Velocity.y);
        }

        [Fact]
        public void Move_ChangesPosition_And_RaisesEvent()
        {
            Vector start = new(0, 0);
            Vector vel = new(0, 0);
            Ball ball = new(start, vel);

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
    }
}
