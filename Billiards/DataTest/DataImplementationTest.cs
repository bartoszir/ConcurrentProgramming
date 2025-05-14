using System;
using Billiards.Data;

namespace Billiards.DataImplementationTest
{
    public class DataImplementationTest
    {
        [Fact]
        public async Task BallPositionChangesAfterDelay()
        {
            using var dataLayer = new DataImplementation();
            var positionHistory = new List<IVector>();
            var updated = new ManualResetEventSlim(false);

            dataLayer.Start(1, (startPos, ball) =>
            {
                ball.NewPositionNotification += (sender, pos) =>
                {
                    positionHistory.Add(pos);
                    if (positionHistory.Count >= 2)
                        updated.Set();
                };
            });

            // czekamy max 0.5s na co najmniej dwie zmiany pozycji
            bool hasMoved = updated.Wait(500);

            Assert.True(hasMoved);
            Assert.True(positionHistory.Count >= 2);
            Assert.NotEqual(positionHistory[0], positionHistory[^1]);
        }

        [Fact]
        public async Task DisposingLayerStopsBallUpdates()
        {
            var layer = new DataImplementation();
            int moveEvents = 0;

            layer.Start(1, (startPos, ball) =>
            {
                ball.NewPositionNotification += (sender, pos) =>
                {
                    moveEvents++;
                };
            });

            await Task.Delay(150); // trochę czasu na zarejestrowanie kilku zmian
            layer.Dispose();
            int afterDispose = moveEvents;

            await Task.Delay(150); // teraz zmiany nie powinny już przychodzić

            Assert.Equal(afterDispose, moveEvents);
        }
    }

}