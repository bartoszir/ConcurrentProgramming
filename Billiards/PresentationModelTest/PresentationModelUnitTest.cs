using Billiards.BusinessLogic;

namespace Billiards.Presentation.Model.Test
{
    public class PresentationModelUnitTest
    {
        [Fact]
        public void DisposeTestMethod()
        {
            //UnderneathLayerFixture underneathLayerFixture = new UnderneathLayerFixture();
            //ModelImplementation? newInstance = null;
            //using (newInstance = new(underneathLayerFixture))
            //{
            //    newInstance.CheckObjectDisposed(x => Assert.False(x));
            //    newInstance.CheckUnderneathLayerAPI(x => Assert.Same(underneathLayerFixture, x));
            //    newInstance.CheckBallChangedEvent(x => Assert.True(x));
            //    Assert.False(underneathLayerFixture.Disposed);
            //}
            //newInstance.CheckObjectDisposed(x => Assert.True(x));
            //newInstance.CheckUnderneathLayerAPI(x => Assert.Same(underneathLayerFixture, x));
            //Assert.True(underneathLayerFixture.Disposed);
            //Assert.Throws<ObjectDisposedException>(() => newInstance.Dispose());
        }

        [Fact]
        public void StartTestMethod()
        {
            //UnderneathLayerFixture underneathLayerFixture = new UnderneathLayerFixture();
            //using (ModelImplementation newInstance = new(underneathLayerFixture))
            //{
            //    newInstance.CheckBallChangedEvent(x => Assert.True(x));
            //    var observer = new DummyObserver();
            //    IDisposable subscription = newInstance.Subscribe(observer);
            //    //IDisposable subscription = newInstance.Subscribe(x => { });
            //    newInstance.CheckBallChangedEvent(x => Assert.False(x));
            //    newInstance.Start(10);
            //    Assert.Equal<int>(10, underneathLayerFixture.NumberOfBalls);
            //    subscription.Dispose();
            //    newInstance.CheckBallChangedEvent(x => Assert.True(x));
            }
        }

    //    //private class DummyObserver : IObserver<IBall>
    //    //{
    //    //    public void OnCompleted() { }
    //    //    public void OnError(Exception error) { }
    //    //    public void OnNext(IBall value) { }
    //    //}

    //    //#region testing instrumentation

    //    //private class UnderneathLayerFixture : BusinessLogicAbstractAPI
    //    //{
    //    //    #region testing instrumentation

    //    //    internal bool Disposed = false;
    //    //    internal int NumberOfBalls = 0;

    //    //    #endregion testing instrumentation

    //    //    #region BusinessLogicAbstractAPI

    //    //    public override void Dispose()
    //    //    {
    //    //        Disposed = true;
    //    //    }

    //    //    public override void Start(int numberOfBalls, Action<IPosition, BusinessLogic.IBall> upperLayerHandler)
    //    //    {
    //    //        NumberOfBalls = numberOfBalls;
    //    //        Assert.NotNull(upperLayerHandler);
    //    //    }

    //    //    #endregion BusinessLogicAbstractAPI
    //    //}

    //    //#endregion testing instrumentation
    //}
}