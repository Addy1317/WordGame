﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace BBG
{
	public class Popup : UIMonoBehaviour
	{
		#region Enums

		private enum State
		{
			Shown,
			Hidden,
			Showing,
			Hidding
		}

		#endregion

		#region Inspector Variables

		[SerializeField] private bool canAndroidBackClose = true;

		[Header("Anim Settings")]
		[SerializeField] protected float			animDuration;
		[SerializeField] protected AnimationCurve	animCurve;
		[SerializeField] protected RectTransform	animContainer;

		#endregion

		#region Member Variables

		private bool		isInitialized;
		private State		state;
		private PopupClosed	callback;

		#endregion

		#region Properties

		public bool CanAndroidBackClose { get { return canAndroidBackClose; } }

		#endregion

		#region Delegates

		public delegate void PopupClosed(bool cancelled, object[] outData);

		#endregion

		#region Public Methods

		public virtual void Initialize()
		{
			gameObject.SetActive(false);
			CG.alpha = 0f;
			state = State.Hidden;
		}

		public void Show()
		{
			Show(null, null);
		}

		public bool Show(object[] inData, PopupClosed callback)
		{
			this.callback = callback;

			if (state != State.Hidden)
			{
				return false;
			}

			state = State.Showing;

			// Show the popup object
			gameObject.SetActive(true);

			// Start the popup show animations
			UIAnimation anim = null;

			anim = UIAnimation.Alpha(gameObject, 0f, 1f, animDuration);
			anim.style = UIAnimation.Style.EaseOut;
			anim.startOnFirstFrame = true;
			anim.Play();

			anim					= UIAnimation.ScaleX(animContainer, 0f, 1f, animDuration);
			anim.style				= UIAnimation.Style.Custom;
			anim.animationCurve		= animCurve;
			anim.startOnFirstFrame	= true;
			anim.Play();

			anim					= UIAnimation.ScaleY(animContainer, 0f, 1f, animDuration);
			anim.style				= UIAnimation.Style.Custom;
			anim.animationCurve		= animCurve;
			anim.startOnFirstFrame	= true;
			anim.OnAnimationFinished += (GameObject obj) => { state = State.Shown; };
			anim.Play();

			OnShowing(inData);

			return true;
		}

		public void Hide(bool cancelled)
		{
			Hide(cancelled, null);
		}

		public void Hide(bool cancelled, object[] outData)
		{
			switch (state)
			{
				case State.Hidden:
				case State.Hidding:
					return;
				case State.Showing:
					UIAnimation.DestroyAllAnimations(gameObject);
					UIAnimation.DestroyAllAnimations(animContainer.gameObject);
					break;
			}

			state = State.Hidding;

			// Start the popup hide animations
			UIAnimation anim = null;

			anim = UIAnimation.Alpha(gameObject, 1f, 0f, animDuration);
			anim.style = UIAnimation.Style.EaseOut;
			anim.startOnFirstFrame = true;

			anim.OnAnimationFinished += (GameObject target) => 
			{
				state = State.Hidden;

				gameObject.SetActive(false);
			};

			anim.Play();

			callback?.Invoke(cancelled, outData);

			OnHiding();
		}

		public virtual void OnShowing(object[] inData)
		{

		}

		public virtual void OnHiding()
		{
			PopupManager.Instance.OnPopupHiding(this);
		}

		#endregion
	}
}