﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BBG.WordBlocks
{
	public class ExtraWordsPopup : Popup
	{
		#region Inspector Variables

		[Space]

		[SerializeField] private Text		rewardAmountText		= null;
		[SerializeField] private Slider		progressSlider			= null;
		[SerializeField] private GameObject	noWordsContainer		= null;
		[SerializeField] private Transform	extraWordsListContainer	= null;
		[SerializeField] private Text		extraWordsTextPrefab	= null;

		#endregion

		#region Member Variables

		private ObjectPool extraWordsTextPool;

		#endregion

		#region Public Methods

		public override void Initialize()
		{
			base.Initialize();

			extraWordsTextPool = new ObjectPool(extraWordsTextPrefab.gameObject, 1, extraWordsListContainer);
		}

		public override void OnShowing(object[] inData)
		{
			int				rewardAmount	= (int)inData[0];
			float			progress		= (float)inData[1];
			List<string>	extraWords		= new List<string>((HashSet<string>)inData[2]);

			extraWords.Sort();

			progressSlider.value	= progress;
			rewardAmountText.text	= "x" + rewardAmount;

			SetupExtraWordsList(extraWords);
		}

		#endregion

		#region Private Methods

		private void SetupExtraWordsList(List<string> extraWords)
		{
			noWordsContainer.SetActive(extraWords.Count == 0);

			extraWordsTextPool.ReturnAllObjectsToPool();

			for (int i = 0; i < extraWords.Count; i++)
			{
				string	word		= extraWords[i];
				Text	wordText	= extraWordsTextPool.GetObject<Text>();

				wordText.text = word;
			}
		}

		#endregion
	}
}
