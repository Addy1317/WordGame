using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if BBG_MT_ADS
using BBG.MobileTools;
#endif

namespace BBG.WordBlocks
{
	[RequireComponent(typeof(Button))]
	public class RewardAdButton : MonoBehaviour
	{
		#region Inspector Variables

		[SerializeField] private int	coinsToReward;
		[SerializeField] private bool	testMode;

		#endregion

		#region Properties

		public Button Button { get { return gameObject.GetComponent<Button>(); } }

		#endregion

		#region Unity Methods

		private void Awake()
		{
			Button.onClick.AddListener(OnClick);

			gameObject.SetActive(false);

			#if BBG_MT_ADS
			MobileAdsManager.Instance.OnRewardAdLoaded	+= OnRewardAdLoaded;
			MobileAdsManager.Instance.OnAdsRemoved		+= OnAdsRemoved;
			#endif

			if (testMode)
			{
				gameObject.SetActive(true);
			}
		}

		#endregion

		#region Private Methods

		private void OnClick()
		{
			if (testMode)
			{
				OnRewardAdGranted();

				return;
			}

			#if BBG_MT_ADS
			if (MobileAdsManager.Instance.RewardAdState != AdNetworkHandler.AdState.Loaded)
			{
				gameObject.SetActive(false);

				Debug.LogError("[RewardAdButton] The reward button was clicked but there is no ad loaded to show.");

				return;
			}

			MobileAdsManager.Instance.ShowRewardAd(OnRewardAdClosed, OnRewardAdGranted);
			#endif
		}

		private void OnRewardAdLoaded()
		{
			gameObject.SetActive(true);
		}

		private void OnRewardAdClosed()
		{
			gameObject.SetActive(false);
		}

		private void OnRewardAdGranted()
		{
			// Give the coins right away
			GameManager.Instance.AddCoins(coinsToReward, false);

			// Show the popup to the user so they know they got the coins
			PopupManager.Instance.Show("reward_ad_granted", new object[] { coinsToReward } );
		}

		private void OnAdsRemoved()
		{
			#if BBG_MT_ADS
			MobileAdsManager.Instance.OnRewardAdLoaded -= OnRewardAdLoaded;
			#endif
			gameObject.SetActive(false);
		}

		#endregion
	}
}
