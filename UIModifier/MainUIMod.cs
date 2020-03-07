using System.Reflection;
using BepInEx;
using UnityEngine;
using UnityEngine.UI;

/*
	Disclaimer:
	Despite it all
	I have no idea what I'm doing to be very honest.
*/

#region TODO:
	/*
	 
	*/
#endregion

namespace UIModifier
{
	
	[R2API.Utils.R2APISubmoduleDependency("ResourcesAPI")]
	[BepInPlugin("com.ohway.UIMod", "UI Modifier", "1.0")]
	public class MainUIMod : BaseUnityPlugin
	{
		public GameObject ModCanvas = null;

		#region Health Bar GameObjects
		// If you're looking for Exp Bar GameObjects, look downwards
		private GameObject ModHealthBarRoot = null;
		public GameObject ModHealthBarReference = null;
		public GameObject ModHealthBarDecoration = null;
		public GameObject ModHealthShrunkenRoot = null;
		public GameObject ModHealthSlashText = null;
		public GameObject VanillaHPBarRef = null;
		#endregion
		public string assetPrefix = "@ohwayUIMod";
		public string bundleName = "textures";
		public string Path_CircularMask = "Assets/textures/circularMask.png";
		public string Path_HealthGlobeDecoration = "Assets/textures/hpOverlay.png";

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
			On.RoR2.UI.ExpBar.Awake += ExpBarAwakeAddon;
		}

		private void HealthBarAwakeAddon(On.RoR2.UI.HealthBar.orig_Awake orig, RoR2.UI.HealthBar self)
		{
			orig(self);
			var currentRect = self.gameObject.GetComponentsInChildren<RectTransform>();
			if (currentRect != null && VanillaHPBarRef == null)
			{
				for (int i = 0; i < currentRect.Length; ++i)
				{
					if (currentRect[i].name == "HealthbarRoot")
					{
						VanillaHPBarRef = currentRect[i].gameObject;
						MainHPGlobeStart();
					}
				}
			}
		}

		private void MainHPGlobeStart()
		{
			if (VanillaHPBarRef != null)
			{
				ModCanvas = Instantiate(new GameObject("UIModifierCanvas"));
				ModCanvas.AddComponent<Canvas>();
				ModCanvas.AddComponent<CanvasScaler>();
				ModCanvas.AddComponent<GraphicRaycaster>();

				Destroy(GameObject.Find("BarRoots").GetComponent<Image>());
				ModHealthBarRoot = Instantiate(new GameObject("HealthGlobeRoot"));
				ModHealthBarReference = Instantiate(new GameObject("HealthGlobe"));
				ModHealthBarDecoration = Instantiate(new GameObject("HealthGlobeBacking"));

				ModHealthBarReference.AddComponent<RectTransform>();
				ModHealthBarReference.AddComponent<Image>();
				ModHealthBarReference.GetComponent<Image>().sprite = Resources.Load<Sprite>(ResPath(Path_CircularMask));
				ModHealthBarReference.AddComponent<Mask>();
				ModHealthBarReference.GetComponent<Mask>().showMaskGraphic = false;

				ModHealthBarDecoration.AddComponent<RectTransform>();
				ModHealthBarDecoration.AddComponent<Image>();
				ModHealthBarDecoration.GetComponent<Image>().sprite = Resources.Load<Sprite>(ResPath(Path_HealthGlobeDecoration));

				ModHealthBarRoot.AddComponent<RectTransform>();

				Destroy(VanillaHPBarRef.GetComponent<Image>());
				ModHealthShrunkenRoot = VanillaHPBarRef.transform.GetChild(0).gameObject;
				ModHealthSlashText = VanillaHPBarRef.transform.GetChild(1).gameObject;

				ModHealthBarRoot.transform.SetParent(ModHealthShrunkenRoot.transform.root);
				ModHealthBarReference.transform.position = ModHealthShrunkenRoot.transform.position;
				ModHealthBarReference.transform.SetParent(ModHealthBarRoot.transform.root);
				ModHealthBarDecoration.transform.position = ModHealthBarReference.transform.position;
				ModHealthBarDecoration.transform.SetParent(ModHealthBarRoot.transform.root);
				ModHealthShrunkenRoot.transform.SetParent(ModHealthBarReference.transform);
				ModHealthSlashText.transform.SetParent(ModHealthBarReference.transform.parent);

				ModHealthShrunkenRoot.GetComponent<RectTransform>().anchorMin = Vector2.zero;
				ModHealthShrunkenRoot.GetComponent<RectTransform>().anchorMax = Vector2.one;
				ModHealthShrunkenRoot.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 100);

				ModHealthBarRoot.GetComponent<RectTransform>().anchorMin = Vector2.zero;
				ModHealthBarRoot.GetComponent<RectTransform>().anchorMax = Vector2.one;
				ModHealthBarRoot.GetComponent<RectTransform>().sizeDelta = Vector2.zero;

				ModHealthBarReference.GetComponent<RectTransform>().anchorMin = Vector2.zero;
				ModHealthBarReference.GetComponent<RectTransform>().anchorMax = Vector2.zero;
				ModHealthBarReference.GetComponent<RectTransform>().sizeDelta = new Vector2(229, 229);
				ModHealthBarReference.GetComponent<RectTransform>().anchoredPosition = new Vector2(100, 110);

				ModHealthBarDecoration.GetComponent<RectTransform>().anchorMin = Vector2.zero;
				ModHealthBarDecoration.GetComponent<RectTransform>().anchorMax = Vector2.zero;
				ModHealthBarDecoration.GetComponent<RectTransform>().sizeDelta = new Vector2(250, 250);
				ModHealthBarDecoration.GetComponent<RectTransform>().anchoredPosition = new Vector2(100, 110);

				ModHealthSlashText.GetComponent<RectTransform>().anchorMin = Vector2.zero;
				ModHealthSlashText.GetComponent<RectTransform>().anchorMax = Vector2.zero;
				ModHealthSlashText.GetComponent<RectTransform>().pivot = Vector2.zero;
				ModHealthSlashText.GetComponent<RectTransform>().anchoredPosition = new Vector2(100, 250);

				ModHealthBarReference.transform.Rotate(0f, 0f, 90f);
				ModHealthBarDecoration.transform.SetSiblingIndex(1);
				ModHealthBarReference.transform.SetSiblingIndex(0);
				ModHealthBarRoot.transform.SetAsFirstSibling();
			}
			else
			{
				Debug.LogError("Attmepted to start without any health bar reference!");
			}
		}

		#region Exp bar GameObjects (DOES NOTHING FOR NOW)
		public GameObject VanillaExpBarRoot = null;
		public GameObject ModExpShrunkenRoot = null;
		public GameObject ModExpBarGroup = null;
		#endregion
		public void ExpBarAwakeAddon(On.RoR2.UI.ExpBar.orig_Awake orig, RoR2.UI.ExpBar self)
		{
			orig(self);
			var currentRect = self.gameObject.GetComponentsInChildren<RectTransform>();
			if (currentRect != null && VanillaExpBarRoot == null)
			{
				for (int i = 0; i < currentRect.Length; ++i)
				{
					if (currentRect[i].name == "ExpBarRoot")
					{
						//VanillaExpBarRoot = currentRect[i].gameObject;
						//MainExpBarStart();
					}
				}
			}
		}

		private void MainExpBarStart()
		{
			ModExpShrunkenRoot = VanillaExpBarRoot.transform.GetChild(0).gameObject;

			ModExpBarGroup = Instantiate(new GameObject("XPBarGroup"));
			ModExpBarGroup.AddComponent<RectTransform>();

			ModExpBarGroup.transform.SetParent(VanillaExpBarRoot.transform.root);
			ModExpShrunkenRoot.transform.SetParent(ModExpBarGroup.transform);

			ModExpBarGroup.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0f);
			ModExpBarGroup.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0f);
			ModExpBarGroup.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 30f);

			ModExpShrunkenRoot.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0f);
			ModExpShrunkenRoot.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0f);
			ModExpShrunkenRoot.GetComponent<RectTransform>().sizeDelta = new Vector2(300f, 10f);
			ModExpShrunkenRoot.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);
		}

		void OnGUI()
		{
		}

		void OnDestroy()
		{
			On.RoR2.UI.HealthBar.Awake -= HealthBarAwakeAddon;
			On.RoR2.UI.ExpBar.Awake -= ExpBarAwakeAddon;
			Destroy(ModHealthBarReference);
			Destroy(ModHealthBarDecoration);
			Destroy(ModHealthBarRoot);
		}
	}
}
