using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BBG.WordBlocks
{
	public class DailyGiftManager : SaveableManager<DailyGiftManager>
	{
		#region Classes

		[System.Serializable]
		public class GiftInfo
		{
			public string	giftId;
			public int		giftAmount;
			public Sprite	giftImage;
		}

		#endregion

		#region Inspector Variables

		[SerializeField] private bool			debugShowPopup	= false;
		[SerializeField] private List<GiftInfo> giftInfos		= null;

		#endregion

		#region Member Variables

		private string lastGiftTimestamp;

		#endregion

		#region Properties

		public override string SaveId { get { return "daily_gift_manager"; } }

		public bool Debug_ShowPopup { get { return Debug.isDebugBuild && debugShowPopup; } }

		#endregion

		#region Unity Methods

		protected override void Awake()
		{
			base.Awake();

			InitSave();
		}

		private void Start()
		{
			// Check if the player needs to be given a daily gift
			if (CheckTimestamp() || Debug_ShowPopup)
			{
				PopupManager.Instance.Show("daily_gift");
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Picks a random gift and gives it to the player, returns the gift that was given
		/// </summary>
		public GiftInfo PickGift()
		{
			// Pick a random gift
			GiftInfo randomGift = giftInfos[Random.Range(0, giftInfos.Count)];

			// Give the gift to the player
			if (randomGift.giftId == "coin")
			{
				GameManager.Instance.AddCoins(randomGift.giftAmount, false);
			}
			else
			{
				GameManager.Instance.AddHint(randomGift.giftId, randomGift.giftAmount);
			}

			SetTimestamp();

			return randomGift;
		}

		#endregion

		#region Private Methods

		private void SetTimestamp()
		{
			lastGiftTimestamp = System.DateTime.UtcNow.ToString("YYYYMMdd");
		}

		private bool CheckTimestamp()
		{
			System.DateTime last	= System.DateTime.ParseExact(lastGiftTimestamp, "YYYYMMdd", null);
			System.DateTime now		= System.DateTime.UtcNow;

			return (now - last).TotalDays >= 1;
		}

		#endregion

		#region Save Methods

		public override Dictionary<string, object> Save()
		{
			Dictionary<string, object> json = new Dictionary<string, object>();

			json["timestamp"] = lastGiftTimestamp;

			return json;
		}

		protected override void LoadSaveData(bool exists, JSONNode saveData)
		{
			if (!exists)
			{
				SetTimestamp();
				return;
			}

			lastGiftTimestamp = saveData["timestamp"].Value;
		}

		#endregion
	}
}
