using System.Diagnostics;
using UnderneathLayerAPI = Billiards.Data.DataAbstractAPI;

namespace Billiards.BusinessLogic
{
	internal class BusinessLogicImplementation : BusinessLogicAbstractAPI
	{
		#region ctor

		public BusinessLogicImplementation() : this(null)
		{ }

		internal BusinessLogicImplementation(UnderneathLayerAPI? underneathLayer)
		{
			layerBellow = underneathLayer == null ? UnderneathLayerAPI.GetDataLayer() : underneathLayer;
		}

		#endregion ctor

		#region BusinessLogicAbstractAPI

		public override void Dispose()
		{
			if (Disposed)
				throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
			layerBellow.Dispose();
			Disposed = true;
		}

		public override void Start(int numberOfBalls, Action<IPosition, IBall> upperLayerHandler)
		{
			if (Disposed)
				throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
			if (upperLayerHandler == null)
				throw new ArgumentNullException(nameof(upperLayerHandler));
			layerBellow.Start(numberOfBalls, (startingPosition, databall) => upperLayerHandler(new Position(startingPosition.x, startingPosition.y), new Ball(databall)));
		}

		#endregion BusinessLogicAbstractAPI

		#region private

		private bool Disposed = false;

		private readonly UnderneathLayerAPI layerBellow;

		#endregion private

		#region TestingInfrastructure

		[Conditional("DEBUG")]
		internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
		{
			returnInstanceDisposed(Disposed);
		}

		#endregion TestingInfrastructure
	}
}