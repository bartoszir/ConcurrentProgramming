using System;
using Xunit;
using System.Collections.Generic;
using BusinessBall = Billiards.BusinessLogic.IBall;
using BusinessVector = Billiards.BusinessLogic.IPosition;
using DataBall = Billiards.Data.IBall;
using DataVector = Billiards.Data.IVector;
using Billiards.BusinessLogic;
using Billiards.Data;

namespace Billiards.BusinessLogicTest
{
    public class BusinessBallTests
    {
        [Fact]
        public void Ball_RaisesPositionChangedEvent_WhenDataBallRaisesEvent()
        {
            // Arrange
            var mockDataBall = new MockDataBall();
            var businessBall = new Ball(mockDataBall);

            int callbackCount = 0;
            BusinessVector? receivedPosition = null;

            businessBall.NewPositionNotification += (sender, position) =>
            {
                callbackCount++;
                receivedPosition = position;
            };

            // Act
            mockDataBall.RaiseEvent(new VectorFixture(3.5, 7.2));

            // Assert
            Assert.Equal(1, callbackCount);
            Assert.NotNull(receivedPosition);
            Assert.Equal(3.5, receivedPosition.x);
            Assert.Equal(7.2, receivedPosition.y);
        }

        private class MockDataBall : DataBall
        {
            public DataVector Velocity { get; set; } = new VectorFixture(0, 0);
            public event EventHandler<DataVector>? NewPositionNotification;
            public double Diameter => 20.0;
            public void RaiseEvent(DataVector v)
            {
                NewPositionNotification?.Invoke(this, v);
            }
        }

        private class VectorFixture : DataVector
        {
            public VectorFixture(double x, double y)
            {
                this.x = x;
                this.y = y;
            }

            public double x { get; init; }
            public double y { get; init; }
        }
    }

    public class BusinessLogicImplementationTests
    {
        private class FakeDataLayer : DataAbstractAPI
        {
            public override void Dispose() => Disposed = true;
            public override void Start(int numberOfBalls, Action<DataVector, DataBall> upperLayerHandler)
            {
                for (int i = 0; i < numberOfBalls; i++)
                {
                    upperLayerHandler(new VectorFixture(1, 2), new MockDataBall());
                }
            }

            public bool Disposed { get; private set; }
        }

        private class MockDataBall : DataBall
        {
            public DataVector Velocity { get; set; } = new VectorFixture(0, 0);
            public event EventHandler<DataVector>? NewPositionNotification;
            public double Diameter => 20.0;
        }

        private class VectorFixture : DataVector
        {
            public double x { get; init; }
            public double y { get; init; }
            public VectorFixture(double x, double y) => (this.x, this.y) = (x, y);
        }

        [Fact]
        public void Start_InvokesCallbackWithBusinessObjects()
        {
            var logic = new BusinessLogicImplementation(new FakeDataLayer());
            int count = 0;

            logic.Start(2, (pos, ball) =>
            {
                Assert.IsType<Position>(pos);
                Assert.IsType<Ball>(ball);
                count++;
            });

            Assert.Equal(2, count);
        }

        [Fact]
        public void Dispose_MarksInstanceAsDisposed()
        {
            var fakeLayer = new FakeDataLayer();
            var logic = new BusinessLogicImplementation(fakeLayer);

            logic.Dispose();

            Assert.True(fakeLayer.Disposed);
        }
    }

    public class PositionTests
    {
        [Fact]
        public void Constructor_AssignsPropertiesCorrectly()
        {
            var pos = new Position(5.5, 6.6);

            Assert.Equal(5.5, pos.x);
            Assert.Equal(6.6, pos.y);
        }
    }
}