﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BBG.WordBlocks
{
	public class ThemeManager : SaveableManager<ThemeManager>
	{
		#region Classes

		[System.Serializable]
		public class ItemId
		{
			public string	id		= "";
			public Type		type	= Type.Color;
		}

		[System.Serializable]
		public class Theme
		{
			public string			name					= "";
			public Sprite			listItemImage			= null;
			public bool				setActiveThemeInEditor	= false;
			public bool				isLocked				= false;
			public int				coinsToUnlock			= 0;
			public List<ThemeItem>	themeItems				= null;
		}

		[System.Serializable]
		public class ThemeItem
		{
			public Color		color	= Color.white;
			public Sprite		image	= null;
			public GameObject	prefab	= null;
		}

		#endregion

		#region Enums

		public enum Type
		{
			Color,
			Image,
			Prefab
		}

		#endregion

		#region Inspector Variables

		[SerializeField] private bool themesEnabled			= false;
		[SerializeField] private bool debugUnlockAllThemes	= false;

		[SerializeField] private List<ItemId>	ids		= null;
		[SerializeField] private List<Theme>	themes	= null;

		#endregion

		#region Member Variables

		private HashSet<string>			unlockedThemes;
		private List<IThemeBehaviour>	themeBehaviours;

		#endregion

		#region Properties

		public override string SaveId { get { return "theme_manager"; } }

		public bool			Enabled				{ get { return themesEnabled; } }
		public List<Theme>	Themes				{ get { return themes; } }
		public int			ActiveThemeIndex	{ get; private set; }
		public Theme		ActiveTheme			{ get { return themes[ActiveThemeIndex]; } }

		public bool Debug_UnlockAllThemes { get { return Debug.isDebugBuild && debugUnlockAllThemes; } }

		#endregion

		#region Unity Methods

		protected override void Awake()
		{
			base.Awake();

			unlockedThemes	= new HashSet<string>();
			themeBehaviours = new List<IThemeBehaviour>();

			InitSave();

			#if UNITY_EDITOR
			// Check if there is an active theme in editor and if so set the ActiveThemeIndex to it
			for (int i = 0; i < themes.Count; i++)
			{
				if (themes[i].setActiveThemeInEditor)
				{
					ActiveThemeIndex = i;
					break;
				}
			}
			#endif
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Registers a theme behaviour to recieve notifications when the theme changes
		/// </summary>
		public void Register(IThemeBehaviour themeBehaviour)
		{
			themeBehaviours.Add(themeBehaviour);
		}

		/// <summary>
		/// Sets the given theme as the active theme
		/// </summary>
		public void SetActiveTheme(Theme theme)
		{
			SetActiveTheme(themes.IndexOf(theme));
		}

		/// <summary>
		/// Sets the active theme to the given index
		/// </summary>
		public void SetActiveTheme(int themeIndex)
		{
			ActiveThemeIndex = themeIndex;

			for (int i = 0; i < themeBehaviours.Count; i++)
			{
				themeBehaviours[i].NotifyThemeChanged();
			}
		}

		/// <summary>
		/// Gets the theme item with the given id in the active theme
		/// </summary>
		public bool GetThemeItem(string id, out ThemeItem themeItem, out ItemId itemId)
		{
			for (int i = 0; i < ids.Count; i++)
			{
				itemId = ids[i];

				if (id == itemId.id)
				{
					themeItem = ActiveTheme.themeItems[i];

					return true;
				}
			}

			itemId		= null;
			themeItem	= null;

			return false;
		}

		/// <summary>
		/// Ises the locked.
		/// </summary>
		public bool IsThemeLocked(int index)
		{
			return IsThemeLocked(themes[index]);
		}

		/// <summary>
		/// Ises the locked.
		/// </summary>
		public bool IsThemeLocked(Theme theme)
		{
			if (Debug_UnlockAllThemes)
			{
				return false;
			}

			return theme.isLocked && !unlockedThemes.Contains(theme.name);
		}

		/// <summary>
		/// Unlocks the given theme
		/// </summary>
		public void UnlockTheme(Theme theme)
		{
			if (!unlockedThemes.Contains(theme.name))
			{
				unlockedThemes.Add(theme.name);
			}
		}

		#endregion

		#region Save Methods

		public override Dictionary<string, object> Save()
		{
			Dictionary<string, object> json = new Dictionary<string, object>();

			json["active_theme_index"]	= ActiveThemeIndex;
			json["unlocked_themes"]		= new List<string>(unlockedThemes);

			return json;
		}

		protected override void LoadSaveData(bool exists, JSONNode saveData)
		{
			if (!exists)
			{
				ActiveThemeIndex = 0;
				return;
			}

			ActiveThemeIndex = saveData["active_theme_index"].AsInt;

			JSONArray unlockedThemesJson = saveData["unlocked_themes"].AsArray;

			for (int i = 0; i < unlockedThemesJson.Count; i++)
			{
				unlockedThemes.Add(unlockedThemesJson[i].Value);
			}
		}

		#endregion
	}
}
