using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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

	public class DiagnosticLogger
	{
		private readonly string logFilePath = "C:\\main\\wspolbiegi2\\diagnostics_logs.txt";
		private readonly Queue<DiagnosticEvent> eventQueue = new Queue<DiagnosticEvent>();
		private readonly object queueLock = new object();
		private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

		public DiagnosticLogger()
		{
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
						// Brak przepustowoœci – spróbuj ponownie póŸniej
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
	}

}