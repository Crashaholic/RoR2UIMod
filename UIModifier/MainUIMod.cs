using System;
using BepInEx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using R2API;
using RoR2.UI;
using RoR2;
using On.RoR2.UI;

/*
	Disclaimer:
	Despite it all
	I have no idea what I'm doing to be very honest.
*/

namespace UIModifier
{
	[BepInPlugin("com.ohway.UIMod", "UI Modifier", "1.0")]
	public class MainUIMod : BaseUnityPlugin
	{
		private GameObject HealthBarReference = null;
		private GameObject MainHPBarRef = null;
		private RectTransform[] rectTransforms = null;

		void Awake()
		{
			On.RoR2.UI.HealthBar.Awake += HealthBarAwakeAddon;
		}

		private void HealthBarAwakeAddon(On.RoR2.UI.HealthBar.orig_Awake orig, RoR2.UI.HealthBar self)
		{
			orig(self);
			var currentRect = self.gameObject.GetComponentsInChildren<RectTransform>();
			if (currentRect != null && MainHPBarRef == null)
			{
				rectTransforms = currentRect;
				for (int i = 0; i < rectTransforms.Length; ++i)
				{
					if (rectTransforms[i].name == "HealthbarRoot")
					{
						MainHPBarRef = rectTransforms[i].gameObject;
						MainHPOrbStart();
					}
				}
			}
		}

		void OnGUI()
		{
		}

		private void MainHPOrbStart()
		{
			if (MainHPBarRef != null)
			{
				HealthBarReference = Instantiate(new GameObject("HealthGlobe"));
				HealthBarReference.AddComponent<RectTransform>();
				HealthBarReference.AddComponent<Image>();
				HealthBarReference.GetComponent<Image>().sprite = Resources.Load<Sprite>("textures/itemicons/texShinyPearlIcon");
				HealthBarReference.AddComponent<Mask>();
				HealthBarReference.GetComponent<Mask>().showMaskGraphic = false;
				for (int i = 0; i < MainHPBarRef.transform.childCount; ++i)
				{
					if (MainHPBarRef.transform.GetChild(i).name == "ShrunkenRoot")
					{
						MainHPBarRef = MainHPBarRef.transform.GetChild(i).gameObject;
					}
				}
				HealthBarReference.transform.position = MainHPBarRef.transform.position;
				HealthBarReference.transform.parent = MainHPBarRef.transform.parent;
				MainHPBarRef.transform.parent = HealthBarReference.transform;
				HealthBarReference.transform.Rotate(0f, 0f, 90f);
				MainHPBarRef.GetComponent<RectTransform>().sizeDelta = new Vector3(6, 6, 0);
				HealthBarReference.transform.localScale = new Vector3(3, 3, 1);
				HealthBarReference.transform.SetSiblingIndex(0);
			}
			else
			{
				Debug.LogError("Attmepted to start without any health bar reference!");
			}
		}

		void Start()
		{

		}

		void Update()
		{

		}

		void OnDestroy()
		{
			On.RoR2.UI.HealthBar.Awake -= HealthBarAwakeAddon;
		}

		//private void OverrideHealthUpdate(On.RoR2.UI.HealthBar.orig_Update orig, RoR2.UI.HealthBar self)
		//{
		//	//throw new NotImplementedException();
		//	orig(self);
		//
		//}
	}
}
