//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using Xunit;
//using Billiards.Presentation.Model;
//using Billiards.BusinessLogic;

//namespace Billiards.PresentationModelTest
//{
//    public class ModelBallTests
//    {
//        private class DummyLogicBall : IBall
//        {
//            public event EventHandler<IPosition> NewPositionNotification;
//            public void RaiseEvent(IPosition position)
//            {
//                NewPositionNotification?.Invoke(this, position);
//            }
//        }

//        private class DummyPosition : IPosition
//        {
//            public double x { get; init; }
//            public double y { get; init; }
//            public DummyPosition(double x, double y)
//            {
//                this.x = x;
//                this.y = y;
//            }
//        }

//        [Fact]
//        public void Constructor_InitializesProperties()
//        {
//            var logicBall = new DummyLogicBall();
//            double top = 10.0;
//            double left = 5.0;

//            var modelBall = new ModelBall(top, left, logicBall);

//            Assert.Equal(top, modelBall.Top);
//            Assert.Equal(left, modelBall.Left);
//        }

//        [Fact]
//        public void NewPositionNotification_UpdatesTopLeft()
//        {
//            var logicBall = new DummyLogicBall();
//            var modelBall = new ModelBall(0.0, 0.0, logicBall);
//            var newPos = new DummyPosition(42.0, 24.0);
//            bool propertyChangedCalled = false;

//            modelBall.PropertyChanged += (sender, e) => propertyChangedCalled = true;

//            logicBall.RaiseEvent(newPos);

//            Assert.Equal(42.0, modelBall.Left);
//            Assert.Equal(24.0, modelBall.Top);
//            Assert.True(propertyChangedCalled);
//        }
//    }

//    public class ModelImplementationTests
//    {
//        private class DummyBall : IBall
//        {
//            public event EventHandler<IPosition> NewPositionNotification;
//        }

//        private class DummyPosition : IPosition
//        {
//            public double x { get; init; }
//            public double y { get; init; }
//            public DummyPosition(double x, double y)
//            {
//                this.x = x;
//                this.y = y;
//            }
//        }

//        private class ObserverDummy : IObserver<IBall>
//        {
//            public List<IBall> Received = new();
//            public void OnCompleted() { }
//            public void OnError(Exception error) { }
//            public void OnNext(IBall value) => Received.Add(value);
//        }

//        private class FakeLogicAPI : BusinessLogicAbstractAPI
//        {
//            public override void Dispose() => Disposed = true;
//            public bool Disposed { get; private set; } = false;

//            public override void Start(int numberOfBalls, Action<IPosition, IBall> upperLayerHandler)
//            {
//                for (int i = 0; i < numberOfBalls; i++)
//                {
//                    var pos = new DummyPosition(10 + i, 20 + i);
//                    var ball = new DummyBall();
//                    upperLayerHandler(pos, ball);
//                }
//            }
//        }

//        [Fact]
//        public void Start_RaisesBallChangedEvents()
//        {
//            var logic = new FakeLogicAPI();
//            var model = new ModelImplementation(logic);
//            List<IBall> events = new();
//            model.BallChanged += (s, e) => events.Add(e.Ball);

//            model.Start(2);

//            Assert.Equal(2, events.Count);
//        }

//        [Fact]
//        public void Dispose_SetsDisposed()
//        {
//            var logic = new FakeLogicAPI();
//            var model = new ModelImplementation(logic);
//            model.Dispose();

//            Assert.True(logic.Disposed);
//        }

//        [Fact]
//        public void Subscribe_ReceivesNotifications()
//        {
//            var logic = new FakeLogicAPI();
//            var model = new ModelImplementation(logic);
//            var observer = new ObserverDummy();

//            model.Subscribe(observer);
//            model.Start(1);

//            Assert.Single(observer.Received);
//        }
//    }

//    public class BallChangedEventArgsTests
//    {
//        private class DummyBall : IBall
//        {
//            public event EventHandler<IPosition> NewPositionNotification;
//        }

//        [Fact]
//        public void BallProperty_AssignsCorrectly()
//        {
//            var dummy = new DummyBall();
//            var args = new BallChaneEventArgs { Ball = dummy };
//            Assert.Equal(dummy, args.Ball);
//        }
//    }
//}