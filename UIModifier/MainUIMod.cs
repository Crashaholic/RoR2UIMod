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
		public string assetPrefix = "@ohwayUIMod";
		public string bundleName = "textures";
		public string Path_CircularMask = "Assets/textures/circularMask.png";
		public string Path_HealthGlobeDecoration = "Assets/textures/hpOverlay.png";

		#region Health Bar GameObjects
		// If you're looking for Exp Bar GameObjects, look downwards
		private GameObject ModHealthBarRoot = null;
		public GameObject ModHealthBarReference = null;
		public GameObject ModHealthBarDecoration = null;
		public GameObject ModHealthShrunkenRoot = null;
		public GameObject ModHealthSlashText = null;
		public GameObject VanillaHPBarRef = null;
		#endregion

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
			//On.RoR2.LocalUserManager.
		}

		private void SetUpModCanvas()
		{
			if (ModCanvas == null)
			{
				ModCanvas = new GameObject("UIModifierCanvas");
				ModCanvas.AddComponent<Canvas>();
				ModCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
				if (VanillaExpBarRoot != null)
				{
					ModCanvas.GetComponent<Canvas>().worldCamera = VanillaExpBarRoot.transform.root.gameObject.GetComponent<Canvas>().worldCamera;
				}
				else if (VanillaHPBarRef != null)
				{
					ModCanvas.GetComponent<Canvas>().worldCamera = VanillaHPBarRef.transform.root.gameObject.GetComponent<Canvas>().worldCamera;
				}

				ModCanvas.AddComponent<CanvasScaler>();
				ModCanvas.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
				ModCanvas.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920, 1080);
				ModCanvas.GetComponent<CanvasScaler>().screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
			}
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
				SetUpModCanvas();
				//ModCanvas.transform.SetSiblingIndex(VanillaHPBarRef.transform.root.GetSiblingIndex() - 1);
				

				Destroy(GameObject.Find("BarRoots").GetComponent<Image>());
				ModHealthBarRoot = new GameObject("HealthGlobeRoot");
				ModHealthBarReference = new GameObject("HealthGlobe");
				ModHealthBarDecoration = new GameObject("HealthGlobeBacking");

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

				ModHealthBarRoot.transform.SetParent(ModCanvas.transform);
				ModHealthBarReference.transform.position = ModHealthShrunkenRoot.transform.position;
				ModHealthBarReference.transform.SetParent(ModCanvas.transform);
				ModHealthBarDecoration.transform.position = ModHealthBarReference.transform.position;
				ModHealthBarDecoration.transform.SetParent(ModCanvas.transform);
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
				ModHealthBarReference.GetComponent<RectTransform>().anchoredPosition = new Vector2(ModHealthBarReference.GetComponent<RectTransform>().sizeDelta.x / 2f, ModHealthBarReference.GetComponent<RectTransform>().sizeDelta.y / 2f);

				ModHealthBarDecoration.GetComponent<RectTransform>().anchorMin = Vector2.zero;
				ModHealthBarDecoration.GetComponent<RectTransform>().anchorMax = Vector2.zero;
				ModHealthBarDecoration.GetComponent<RectTransform>().sizeDelta = new Vector2(250, 250);
				ModHealthBarDecoration.GetComponent<RectTransform>().anchoredPosition = new Vector2(ModHealthBarReference.GetComponent<RectTransform>().sizeDelta.x / 2f, ModHealthBarReference.GetComponent<RectTransform>().sizeDelta.y / 2f);

				ModHealthSlashText.GetComponent<RectTransform>().anchorMin = Vector2.zero;
				ModHealthSlashText.GetComponent<RectTransform>().anchorMax = Vector2.zero;
				ModHealthSlashText.GetComponent<RectTransform>().pivot = Vector2.zero;
				ModHealthSlashText.GetComponent<RectTransform>().anchoredPosition = new Vector2(ModHealthBarReference.GetComponent<RectTransform>().sizeDelta.x / 2f, ModHealthBarDecoration.GetComponent<RectTransform>().sizeDelta.y);

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

		#region Exp bar GameObjects
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
						VanillaExpBarRoot = currentRect[i].gameObject;
						MainExpBarStart();
					}
				}
			}
		}

		private void MainExpBarStart()
		{
			if (VanillaExpBarRoot != null)
			{
				SetUpModCanvas();

				ModExpShrunkenRoot = VanillaExpBarRoot.transform.GetChild(0).gameObject;

				ModExpBarGroup = new GameObject("XPBarGroup");

				ModExpBarGroup.transform.SetParent(ModCanvas.transform);
				ModExpShrunkenRoot.transform.SetParent(ModExpBarGroup.transform);
				ModExpBarGroup.transform.position = ModExpShrunkenRoot.transform.position;

				ModExpBarGroup.AddComponent<RectTransform>();
				ModExpBarGroup.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
				ModExpBarGroup.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
				ModExpBarGroup.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);

				ModExpShrunkenRoot.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0f);
				ModExpShrunkenRoot.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0f);
				ModExpShrunkenRoot.GetComponent<RectTransform>().sizeDelta = new Vector2(300f, 1000f);
				ModExpShrunkenRoot.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);

			}
		}

		public GameObject HierachyTracker = null;
		public bool HierachyViewerPanelShow = false;
		public Vector2 scrollPosition = Vector2.zero;
		void OnGUI()
		{
			#region Runtime Hierachy Viewer
			if (HierachyViewerPanelShow)
			{
				if (HierachyTracker == null)
				{
					HierachyTracker = new GameObject("HierachyTracker");
				}
				// HiearchyTracker is at root, so get the scene root objects
				if (HierachyTracker.transform.root == HierachyTracker.transform)
				{
					var allGameObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
					scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(300), GUILayout.Height(500));
					for (int i = 0; i < allGameObjects.Length; ++i)
					{
						if (allGameObjects[i].name.Contains("HierachyTracker")) continue;
						if (GUILayout.Button(allGameObjects[i].name))
						{
							if (allGameObjects[i].transform.childCount > 0)
							{
								HierachyTracker.transform.SetParent(allGameObjects[i].transform);
							}
						}
					}
					GUILayout.EndScrollView();
				}
				else
				{
					if (GUILayout.Button("< Up"))
					{
						if (HierachyTracker.transform.parent.root == HierachyTracker.transform.parent)
						{
							HierachyTracker.transform.SetParent(null);
						}
						else
						{
							HierachyTracker.transform.SetParent(HierachyTracker.transform.parent.parent);
						}
					}
					scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(300), GUILayout.Height(500));
					for (int i = 0; i < HierachyTracker.transform.parent.childCount; ++i)
					{
						if (HierachyTracker.transform.parent.GetChild(i).name.Contains("HierachyTracker")) continue;
						if (GUILayout.Button(HierachyTracker.transform.parent.GetChild(i).name))
						{
							if (HierachyTracker.transform.parent.GetChild(i).childCount > 0)
							{
								HierachyTracker.transform.SetParent(HierachyTracker.transform.parent.GetChild(i));
							}
						}
					}
					GUILayout.EndScrollView();
				}
			}
			#endregion
		}

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.F7))
			{
				HierachyViewerPanelShow = !HierachyViewerPanelShow;
			}
			if (VanillaHPBarRef != null)
			{
				if (RoR2.LocalUserManager.GetFirstLocalUser() != null)
				{
					if (RoR2.LocalUserManager.GetFirstLocalUser().cachedBodyObject != null)
					{
						var body = RoR2.LocalUserManager.GetFirstLocalUser().cachedBody;
						print("Life: " + body.healthComponent.health + "/" + body.healthComponent.fullHealth + "\n Barrier: " + body.healthComponent.barrier);
					}
				}
			}
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
