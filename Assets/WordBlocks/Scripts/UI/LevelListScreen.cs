using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BBG.WordBlocks
{
	public class LevelListScreen : Screen
	{
		#region Inspector Variables

		[Space]

		[SerializeField] private ScrollRect		packListScrollRect	= null;
		[SerializeField] private RectTransform	packListContainer	= null;
		[SerializeField] private PackListItem	packListItemPrefab	= null;
		[SerializeField] private LevelListItem	levelListItemPrefab	= null;
		[SerializeField] private float			expandAnimDuration	= 0.5f;

		#endregion

		#region Member Variables

		private ExpandableListHandler<PackInfo> expandableListHandler;
		private ObjectPool						levelListItemPool;

		#endregion

		#region Public Methods

		public override void Initialize()
		{
			levelListItemPool		= new ObjectPool(levelListItemPrefab.gameObject, 1, ObjectPool.CreatePoolContainer(transform));
			expandableListHandler	= new ExpandableListHandler<PackInfo>(GameManager.Instance.PackInfos, packListItemPrefab, packListContainer, packListScrollRect, expandAnimDuration);

			// Add a listener for when a PackListItem is first created to pass it the level list item pool
			expandableListHandler.OnItemCreated += (ExpandableListItem<PackInfo> packListItem) => 
			{
				(packListItem as PackListItem).SetLevelListItemPool(levelListItemPool);
			};

			expandableListHandler.Setup();
		}

		public override void Show(bool back, bool immediate)
		{
			base.Show(back, immediate);

			if (!back)
			{
				levelListItemPool.ReturnAllObjectsToPool();

				int expandedListItem = 0;
				for (int i = 0; i < GameManager.Instance.PackInfos.Count; i++)
				{
					PackInfo packInfo = GameManager.Instance.PackInfos[i];

					bool isPackLocked = GameManager.Instance.IsLevelLocked(packInfo.FromLevelNumber);
					bool isPackCompeted = packInfo.ToLevelNumber <= GameManager.Instance.LastCompletedLevel;

					if (!isPackLocked && !isPackCompeted)
					{
						expandedListItem = i;
						break;
					}
				}

				expandableListHandler.Reset();
				expandableListHandler.ScrollToMiddle(expandedListItem);
			}
			else
			{
				expandableListHandler.Refresh();
			}
		}

		#endregion
	}
}
