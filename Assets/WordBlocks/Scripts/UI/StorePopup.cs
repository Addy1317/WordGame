using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BBG.WordBlocks
{
	public class StorePopup : Popup
	{
		#region Public Methods

		public override void OnShowing(object[] inData)
		{
			#if BBG_MT_IAP
			BBG.MobileTools.IAPManager.Instance.OnProductPurchased += OnProductPurchases;
			#endif
		}

		public override void OnHiding()
		{
			base.OnHiding();

			#if BBG_MT_IAP
			BBG.MobileTools.IAPManager.Instance.OnProductPurchased -= OnProductPurchases;
			#endif
		}

		#endregion

		#region Private Methods

		private void OnProductPurchases(string productId)
		{
			Hide(false);

			PopupManager.Instance.Show("product_purchased");
		}

		#endregion
	}
}
