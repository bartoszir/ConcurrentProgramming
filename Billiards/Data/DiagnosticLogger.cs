using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Billiards.Data
{
    public class DiagnosticEvent
    {
        public DateTime Timestamp { get; set; }
        public string? EventType { get; set; }
        public int BallId1 { get; set; }
        public int? BallId2 { get; set; }
        public string? Position { get; set; }

        public override string ToString()
        {
            return $"{Timestamp:O};{EventType};{BallId1};{BallId2?.ToString() ?? "null"};{Position}";
        }
    }

    internal class DiagnosticLogger
    {
        private readonly string logFilePath;
        private readonly Queue<DiagnosticEvent> eventQueue = new();
        private readonly object queueLock = new();
        private readonly CancellationTokenSource cancellationTokenSource = new();

        public DiagnosticLogger()
        {
            // Utwórz nazwê pliku na podstawie aktualnej daty i czasu
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            logFilePath = $"C:\\main\\wspolbiegi2\\diagnostics_logs_{timestamp}.txt";

            // (opcjonalnie, do debugowania)
            Console.WriteLine($"[DiagnosticLogger] Logging to: {logFilePath}");

            Task.Run(() => ProcessQueueAsync(cancellationTokenSource.Token));
        }

        public void Log(DiagnosticEvent ev)
        {
            lock (queueLock)
            {
                eventQueue.Enqueue(ev);
            }
        }

        private async Task ProcessQueueAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                DiagnosticEvent? ev = null;

                lock (queueLock)
                {
                    if (eventQueue.Count > 0)
                    {
                        ev = eventQueue.Dequeue();
                    }
                }

                if (ev != null)
                {
                    try
                    {
                        await File.AppendAllTextAsync(logFilePath, ev.ToString() + Environment.NewLine);
                    }
                    catch (IOException)
                    {
                        // Jeœli wyst¹pi problem z plikiem, spróbuj ponownie póŸniej
                        lock (queueLock)
                        {
                            eventQueue.Enqueue(ev);
                        }
                        await Task.Delay(500);
                    }
                }
                else
                {
                    await Task.Delay(100);
                }
            }
        }

        public void Stop()
        {
            cancellationTokenSource.Cancel();
        }

        // Metoda pomocnicza np. do logowania kolizji
        public void LogCollisionWithWall(int ballId, string wallName, double x, double y)
        {
            Log(new DiagnosticEvent
            {
                Timestamp = DateTime.Now,
                EventType = $"WallHit ({wallName})",
                BallId1 = ballId,
                BallId2 = null,
                Position = $"x={Math.Round(x, 2)},y={Math.Round(y, 2)}"
            });
        }

        public void LogCollisionWithBall(Ball ball1, Ball ball2)
        {
            Log(new DiagnosticEvent
            {
                Timestamp = DateTime.Now,
                EventType = "Collision",
                BallId1 = ball1.Id,
                BallId2 = ball2.Id,
                Position = $"x={Math.Round(ball1.Position.x, 2)},y={Math.Round(ball2.Position.y, 2)}"
            });
        }
    }
}
