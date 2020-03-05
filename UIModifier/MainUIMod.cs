using System;
using System.Reflection;
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
	
	[R2API.Utils.R2APISubmoduleDependency("ResourcesAPI")]
	[BepInPlugin("com.ohway.UIMod", "UI Modifier", "1.0")]
	public class MainUIMod : BaseUnityPlugin
	{
		private GameObject ModHealthBarReference = null;
		private GameObject ModHealthBarDecoration = null;
		private GameObject ModShrunkenRoot = null;
		private GameObject MainHPBarRef = null;
		private RectTransform[] rectTransforms = null;
		private string assetPrefix = "@ohwayUIMod";
		private string bundleName = "textures";
		private string Path_CircularMask = "Assets/textures/circularMask.png";
		private string Path_Decoration = "Assets/textures/hpOverlay.png";

		private string ResPath(string assetPath) => assetPrefix + ':' + assetPath;

		void Awake()
		{
			using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("UIModifier." + bundleName))
			{
				var bundle = AssetBundle.LoadFromStream(stream);
				var provider = new R2API.AssetBundleResourcesProvider(assetPrefix, bundle);
				R2API.ResourcesAPI.AddProvider(provider);
			}
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
			var strings = Assembly.GetExecutingAssembly().GetManifestResourceNames();
			for (int i = 0; i < strings.Length; i++)
			{
				GUILayout.Label(strings[i]);
			}
		}

		private void MainHPOrbStart()
		{
			if (MainHPBarRef != null)
			{
				ModHealthBarReference = Instantiate(new GameObject("HealthGlobe"));
				ModHealthBarDecoration = Instantiate(new GameObject("HealthGlobeBacking"));
				ModHealthBarReference.AddComponent<RectTransform>();
				ModHealthBarReference.AddComponent<Image>();
				ModHealthBarReference.GetComponent<Image>().sprite = Resources.Load<Sprite>(ResPath(Path_CircularMask));
				ModHealthBarReference.AddComponent<Mask>();
				ModHealthBarReference.GetComponent<Mask>().showMaskGraphic = false;
				ModHealthBarDecoration.AddComponent<RectTransform>();
				ModHealthBarDecoration.AddComponent<Image>();
				ModHealthBarDecoration.GetComponent<Image>().sprite = Resources.Load<Sprite>(ResPath(Path_Decoration));
				Destroy(MainHPBarRef.GetComponent<Image>());
				ModShrunkenRoot = MainHPBarRef.transform.GetChild(0).gameObject;
				
				ModHealthBarReference.transform.position = ModShrunkenRoot.transform.position;
				ModHealthBarReference.transform.SetParent(ModShrunkenRoot.transform.parent.parent.parent.parent.parent);
				ModHealthBarDecoration.transform.position = ModHealthBarReference.transform.position;
				ModHealthBarDecoration.transform.SetParent(ModShrunkenRoot.transform.parent.parent.parent.parent.parent);
				ModShrunkenRoot.transform.SetParent(ModHealthBarReference.transform);

				ModShrunkenRoot.GetComponent<RectTransform>().anchorMin = Vector2.zero;
				ModShrunkenRoot.GetComponent<RectTransform>().anchorMax = Vector2.one;
				ModShrunkenRoot.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 100);

				ModHealthBarReference.GetComponent<RectTransform>().anchorMin = Vector2.zero;
				ModHealthBarReference.GetComponent<RectTransform>().anchorMax = Vector2.zero;
				ModHealthBarReference.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 200);
				ModHealthBarReference.GetComponent<RectTransform>().anchoredPosition = new Vector2(100, 100);

				ModHealthBarDecoration.GetComponent<RectTransform>().anchorMin = Vector2.zero;
				ModHealthBarDecoration.GetComponent<RectTransform>().anchorMax = Vector2.zero;
				ModHealthBarDecoration.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 200);
				ModHealthBarDecoration.GetComponent<RectTransform>().anchoredPosition = new Vector2(100, 100);

				ModHealthBarReference.transform.Rotate(0f, 0f, 90f);
				ModHealthBarDecoration.transform.SetSiblingIndex(1);
				ModHealthBarReference.transform.SetSiblingIndex(0);
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
			Destroy(ModHealthBarReference);
			Destroy(ModHealthBarDecoration);
		}
	}
}
