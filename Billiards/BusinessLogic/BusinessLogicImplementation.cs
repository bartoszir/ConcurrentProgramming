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

		public override void Start(int numberOfBalls, double tableWidth, double tableHeight)
		{
			TableWidth = tableWidth;
			TableHeight = tableHeight;
			//layerBellow.SetTableSize(tableWidth, tableHeight);
			layerBellow.TableWidth = tableWidth;
			layerBellow.TableHeight = tableHeight;

			Start(numberOfBalls, _upperLayerHandler!);
		}

		public override void Start(int numberOfBalls, Action<IPosition, IBall> upperLayerHandler)
		{
			if (Disposed)
				throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
			if (upperLayerHandler == null)
				throw new ArgumentNullException(nameof(upperLayerHandler));
			//layerBellow.SetTableSize(tableWidth, tableHeight);
			_upperLayerHandler = upperLayerHandler;
			layerBellow.Start(numberOfBalls, (startingPosition, databall) => upperLayerHandler(new Position(startingPosition.x, startingPosition.y), new Ball(databall)));
		}

        public override void Start(int numberOfBalls, double tableWidth, double tableHeight, Action<IPosition, IBall> upperLayerHandler)
        {
			TableWidth = tableWidth;
			TableHeight = tableHeight;

			//layerBellow.TableWidth = tableWidth;
			//layerBellow.TableHeight = tableHeight;
			SetTableSize(tableWidth, tableHeight);

			_upperLayerHandler = upperLayerHandler;

			layerBellow.Start(numberOfBalls, (startingPosition, databall) => upperLayerHandler(new Position(startingPosition.x, startingPosition.y), new Ball(databall)));
        }


        public override void SetTableSize(double width, double height)
		{
            layerBellow.SetTableSize(width, height);
        }

		public override double TableWidth { get; set; }
		public override double TableHeight { get; set; }

        #endregion BusinessLogicAbstractAPI

        #region private

        private bool Disposed = false;

		private readonly UnderneathLayerAPI layerBellow;

		private Action<IPosition, IBall>? _upperLayerHandler;

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