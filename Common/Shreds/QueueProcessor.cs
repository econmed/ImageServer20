using System;
using System.Collections.Generic;
using System.Threading;

namespace ClearCanvas.Common.Shreds
{
	/// <summary>
	/// Abstract base class for queue processor classes.
	/// </summary>
	/// <remarks>
	/// </remarks>
	public abstract class QueueProcessor
	{
		private volatile bool _stopRequested;

		/// <summary>
		/// Constructor.
		/// </summary>
		protected QueueProcessor()
		{
		}

		/// <summary>
		/// Runs the processor.
		/// </summary>
		/// <remarks>
		/// This method is expected to block indefinitely until the <see cref="RequestStop"/>
		/// method is called, at which point it should exit in a timely manner.
		/// </remarks>
		public void Run()
		{
			RunCore();
		}

		/// <summary>
		/// Requests the task to exit gracefully.
		/// </summary>
		/// <remarks>
		/// This method will be called on a thread other than the thread on which the task is executing.
		/// This method should return quickly - it should not block.  A typical implementation simply
		/// sets a flag that causes the <see cref="Run"/> method to terminate.
		/// must be implemented in such a way as to heed
		/// a request to stop within a timely manner.
		/// </remarks>
		public void RequestStop()
		{
			_stopRequested = true;
		}

		/// <summary>
		/// Implements the main logic of the processor.
		/// </summary>
		/// <remarks>
		/// Implementation is expected to run indefinitely but must poll the
		/// <see cref="StopRequested"/> property and exit in a timely manner when true.
		/// </remarks>
		protected abstract void RunCore();

		/// <summary>
		/// Gets a value indicating whether this processor has been requested to terminate.
		/// </summary>
		protected bool StopRequested
		{
			get { return _stopRequested; }
		}
	}

	/// <summary>
	/// Abstract base class for queue processor classes.
	/// </summary>
	/// <typeparam name="TItem"></typeparam>
	/// <remarks>
	/// <para>
	/// This class implements the logic to process a queue of items.  It polls the queue
	/// for a batch of items to process, processes those items, and then polls the queue
	/// again.  If the queue is empty, it sleeps for a preset amount of time.
	/// </para>
	/// <para>
	/// All threading is handled externally by <see cref="QueueProcessorShred"/>.
	/// </para>
	/// </remarks>
	public abstract class QueueProcessor<TItem> : QueueProcessor
	{
		private const int SnoozeIntervalInMilliseconds = 100;

		private readonly int _batchSize;
		private TimeSpan _sleepTime;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="batchSize">Max number of items to pull off queue for processing.</param>
		/// <param name="sleepTime"></param>
		protected QueueProcessor(int batchSize, TimeSpan sleepTime)
		{
			_batchSize = batchSize;
			_sleepTime = sleepTime;
		}

		/// <summary>
		/// Gets the next batch of items from the queue.
		/// </summary>
		/// <param name="batchSize"></param>
		/// <returns></returns>
		protected abstract IList<TItem> GetNextBatch(int batchSize);

		/// <summary>
		/// Called to process a queue item.
		/// </summary>
		/// <param name="item"></param>
		protected abstract void ProcessItem(TItem item);

		#region Override Methods

		/// <summary>
		/// Implements the main logic of the processor.
		/// </summary>
		/// <remarks>
		/// Implementation is expected to run indefinitely but must poll the
		/// StopRequested property and exit in a timely manner when true.
		/// </remarks>
		protected override void RunCore()
		{
			while (!StopRequested)
			{
				IList<TItem> items = GetNextBatch(_batchSize);

				// if no items, sleep
				if (items.Count == 0 && !StopRequested)
				{
					Sleep();
				}
				else
				{
					// process each item
					foreach (TItem item in items)
					{
						// break if stop requested
						// (unprocessed items will remain in queue and be picked up next time)
						if (StopRequested)
							break;

						// process the item
						ProcessItem(item);
					}
				}
			}
		}

		#endregion

		#region Helpers

		private void Sleep()
		{
			// sleep for the total sleep time, unless stop requested
			for (int i = 0; i < _sleepTime.TotalMilliseconds
				&& !StopRequested; i += SnoozeIntervalInMilliseconds)
			{
				Thread.Sleep(SnoozeIntervalInMilliseconds);
			}
		}

		#endregion

	}
}