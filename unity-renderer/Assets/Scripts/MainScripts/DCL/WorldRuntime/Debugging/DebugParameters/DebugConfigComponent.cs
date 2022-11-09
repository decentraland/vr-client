using System.Collections.Generic;
using DCL.Components;
using DCL.Interface;
using DCL.SettingsCommon;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Vuplex.WebView;

namespace DCL
{
    public class DebugConfigComponent : MonoBehaviour
    {
        private static DebugConfigComponent sharedInstance;
        
        
        [SerializeField] private GameObject startMenu;
        [SerializeField] private GameObject browserOptionsButton;
        [SerializeField] private TMP_Text browserMessage;
        [SerializeField] private TMP_Text popupMessage;
        [SerializeField] private GameObject popupMessageObj;
        [SerializeField] private CanvasWebViewPrefab DCLWebview;
        [SerializeField] private CanvasKeyboard keyboardDCL;
        [SerializeField] private CanvasWebViewPrefab optionsWeview;
        [SerializeField] public CanvasKeyboard keyboardOptions;
        [SerializeField] private TMP_InputField urlInput;
        [SerializeField] private Button reload;
        [SerializeField] private Button swapTabs;
        [SerializeField] private Toggle useInternalBrowser;
        private string webViewURL = "";
        private bool isMainTab = true;
        
        public static DebugConfigComponent i
        {
            get
            {
                if (sharedInstance == null || sharedInstance.isActiveAndEnabled)
                    sharedInstance = FindObjectOfType<DebugConfigComponent>();

                return sharedInstance;
            }
            private set => sharedInstance = value;
        }

        public DebugConfig debugConfig;

        public enum DebugPanel
        {
            Off,
            Scene,
            Engine
        }

        public enum BaseUrl
        {
            ZONE,
            ORG,
            LOCAL_HOST,
            CUSTOM,
        }

        public enum Network
        {
            MAINNET,
            GOERLI,
        }


        [Header("General Settings")] public bool OpenBrowserOnStart;
        public bool openInternalBrowser;

        public bool webSocketSSL = false;

        [Header("Kernel General Settings")] public string kernelVersion;
        public bool useCustomContentServer = false;

        public string customContentServerUrl = "http://localhost:1338/";

        [Space(10)] public BaseUrl baseUrlMode = BaseUrl.ZONE;
        [DrawIf("baseUrlMode", BaseUrl.CUSTOM)]
        public string customURL = "https://play.decentraland.zone/?";

        [Space(10)] public Network network;

        [Tooltip(
            "Set this field to force the realm (server). On the latin-american zone, recommended realms are fenrir-amber, baldr-amber and thor. Other realms can give problems to debug from Unity editor due to request certificate issues.\n\nFor auto selection leave this field blank.\n\nCheck out all the realms at https://catalyst-monitor.vercel.app/?includeDevServers")]
        public string realm;

        public Vector2 startInCoords = new Vector2(-99, 109);

        [Header("Kernel Misc Settings")] public bool forceLocalComms = true;

        public bool enableTutorial = false;
        public bool builderInWorld = false;

        public int parcelRadiusToLoad = 3;
        public bool disableAssetBundles = true;
        public bool multithreaded = false;

        public bool soloScene = true;
        public bool enableDebugMode = false;

        public DebugPanel debugPanelMode = DebugPanel.Off;

        
        [Header("Performance")]
        public bool disableGLTFDownloadThrottle = false;
        public bool runPerformanceMeterToolDuringLoading = false;
        private PerformanceMeterController performanceMeterController;

        private void Awake()
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            useInternalBrowser.transform.parent.gameObject.SetActive((false));
            #else
            browserOptionsButton.SetActive(false);
            #endif
            
            if (sharedInstance == null)
                sharedInstance = this;
            DontDestroyOnLoad(this);
            DontDestroyOnLoad(DCLWebview);
            DataStore.i.debugConfig.soloScene = debugConfig.soloScene;
            
            
            
            //debugConfig.openInternalBrowser = Settings.i.generalSettings.Data.useInternalBrowser;
            //this.OpenInternalBrowser = debugConfig.openInternalBrowser;
            DataStore.i.debugConfig.soloSceneCoords = debugConfig.soloSceneCoords;
            DataStore.i.debugConfig.ignoreGlobalScenes = debugConfig.ignoreGlobalScenes;
            DataStore.i.debugConfig.msgStepByStep = debugConfig.msgStepByStep;
            DataStore.i.performance.multithreading.Set(multithreaded);
            #if UNITY_ANDROID && !UNITY_EDITOR
            if (disableGLTFDownloadThrottle) DataStore.i.performance.maxDownloads.Set(999);
            #else
            if (disableGLTFDownloadThrottle) DataStore.i.performance.maxDownloads.Set(999);
            #endif
            Texture.allowThreadedTextureCreation = multithreaded;
            // options.Initialized += (sender, eventArgs) =>
            // {
            //     Debug.Log($"Secondary Webview loading {htmlServerTest}");
            //     options.WebView.LoadHtml(htmlServerTest);
            // };
           #if (UNITY_EDITOR  || UNITY_STANDALONE)
            StandaloneWebView.GloballySetUserAgent("Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36");
            StandaloneWebView.SetIgnoreCertificateErrors(true);
            StandaloneWebView.GloballySetUserAgent(false);
            StandaloneWebView.SetCameraAndMicrophoneEnabled(true);
            StandaloneWebView.SetAutoplayEnabled(true);
            StandaloneWebView.SetTargetFrameRate(72);
            StandaloneWebView.SetCommandLineArguments("--disable-web-security");
            optionsWeview.gameObject.SetActive(false);
            keyboardOptions.gameObject.SetActive(false);
#elif UNITY_ANDROID
            AndroidGeckoWebView.GloballySetUserAgent("Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36");
            Web.SetUserAgent("Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36");
            Web.SetStorageEnabled(true); 
            Web.SetIgnoreCertificateErrors(true);
            // Web.EnableRemoteDebugging();
            Web.SetAutoplayEnabled(true);
            AndroidGeckoWebView.SetIgnoreCertificateErrors(true);
            // AndroidGeckoWebView.GloballySetUserAgent(false);
            AndroidGeckoWebView.SetCameraAndMicrophoneEnabled(true);

            AndroidGeckoWebView.SetAutoplayEnabled(true);
            AndroidGeckoWebView.SetStorageEnabled(true);
            AndroidGeckoWebView.SetEnterpriseRootsEnabled(true);
            AndroidGeckoWebView.SetDrmEnabled(true);
            AndroidGeckoWebView.SetPreferences(new Dictionary<string, string> {
                ["network.websocket.allowInsecureFromHTTPS"] = "true",
                ["dom.security.https_only_check_path_upgrade_downgrade_endless_loop"] = "false",
                ["dom.security.https_only_mode_break_upgrade_downgrade_endless_loop"] = "false",
                ["security.csp.enable"] = "false",
                ["dom.webnotifications.allowcrossoriginiframe"] = "true",
                ["dom.webnotifications.allowinsecure"] = "true",
                ["network.auth.subresource-img-cross-origin-http-auth-allow"] = "true",
                ["network.http.referer.XOriginPolicy"] = "1"
                // ["dom.security.https_only_mode_send_http_background_request"] = "false",
                // ["security.allow_unsafe_parent_loads"] = "true",
                // 
                // //["network.dns.echconfig.fallback_to_origin_when_all_failed"] = "false",
                //
                // ["security.fileuri.strict_origin_policy"] = "false",
                // ["dom.cross_origin_iframes_loaded_in_background"] = "true",
                // ["security.csp.enableNavigateTo"] = "true",
                // ["security.mixed_content.block_active_content"] = "	false",
                // ["security.insecure_field_warning.ignore_local_ip_address"] = "false",
                // ["security.mixed_content.upgrade_display_content"] = "false",
                // ["network.websocket.auto-follow-http-redirects"] = "true",
                // 
                
            });
            
#endif


        }

        private void Start()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
//don't have a method of using external browser on quest2.
            openInternalBrowser = true;
#endif
            useInternalBrowser.isOn = openInternalBrowser;
            WebInterface.openURLInternal = openInternalBrowser;
            if (openInternalBrowser)
            {
                browserMessage.transform.parent.gameObject.SetActive(true);
                DCLWebview.gameObject.SetActive((true));
                keyboardDCL.gameObject.SetActive((true));
            }
            else
            {
                
                DCLWebview.gameObject.SetActive((false));
                keyboardDCL.gameObject.SetActive((false));
                
            }
            keyboardOptions.InputReceived += (sender, key) =>
            {
                optionsWeview.WebView.HandleKeyboardInput(key.Value);
            };
            keyboardDCL.InputReceived += (sender, key) =>
            {
                DCLWebview.WebView.HandleKeyboardInput(key.Value);
            };
        
            lock (DataStore.i.wsCommunication.communicationReady)
            {
                if (DataStore.i.wsCommunication.communicationReady.Get())
                {
                    Debug.Log("Debug Config Init");
                    InitConfig();
                }
                else
                {
                    Debug.Log("Debug Config starting listener for OnCommunicationReadyChangedValue");
                    DataStore.i.wsCommunication.communicationReady.OnChange += OnCommunicationReadyChangedValue;
                }
            }
        }



        private void OnCommunicationReadyChangedValue(bool newState, bool prevState)
        {
            Debug.Log($"DebugConfig OnCommunicationReadyChangedValue {newState}");
            if (newState && !prevState)
                InitConfig();
            
            
            //DataStore.i.wsCommunication.communicationReady.OnChange -= OnCommunicationReadyChangedValue;
        }

        private void InitConfig()
        {
            if (useCustomContentServer)
            {
                RendereableAssetLoadHelper.useCustomContentServerUrl = true;
                RendereableAssetLoadHelper.defaultLoadingType = RendereableAssetLoadHelper.LoadingType.ASSET_BUNDLE_WITH_GLTF_FALLBACK;
                RendereableAssetLoadHelper.customContentServerUrl = customContentServerUrl;
            }

            if (OpenBrowserOnStart)
                OpenWebBrowser();

            if (runPerformanceMeterToolDuringLoading)
            {
                CommonScriptableObjects.forcePerformanceMeter.Set(true);
                performanceMeterController = new PerformanceMeterController();
                performanceMeterController.StartSampling(999);
                CommonScriptableObjects.rendererState.OnChange += OnRendererStateChanged;
            }
        }
        private void OnRendererStateChanged(bool current, bool previous)
        {
            CommonScriptableObjects.rendererState.OnChange -= OnRendererStateChanged;
            performanceMeterController.StopSampling();
        }

        private void OpenWebBrowser()
        {
            string baseUrl = "";
            string debugString = "";


            if (baseUrlMode.Equals(BaseUrl.CUSTOM))
            {
                baseUrl = this.customURL;
                if (string.IsNullOrEmpty(this.customURL))
                {
                    Debug.LogError("Custom url cannot be empty");
                    QuitGame();
                    return;
                }
            }
            else if (baseUrlMode.Equals(BaseUrl.LOCAL_HOST))
            {
                baseUrl = "http://localhost:8080/?";
            }
            else if (baseUrlMode.Equals(BaseUrl.ORG))
            {
                baseUrl = "http://play.decentraland.org/?";
                if (!webSocketSSL)
                {
                    Debug.LogError(
                        "play.decentraland.org only works with WebSocket SSL, please change the base URL to play.decentraland.zone");
                    QuitGame();
                    return;
                }
            }
            else
            {
                baseUrl = "http://play.decentraland.zone/?";
            }
         
            switch (network)
            {
                case Network.GOERLI:
                    debugString = "NETWORK=goerli&";
                    break;
                case Network.MAINNET:
                    debugString = "NETWORK=mainnet&";
                    break; 
            }

            if (!string.IsNullOrEmpty(kernelVersion))
            {
                debugString += $"kernel-version={kernelVersion}&";
            }

            if (forceLocalComms)
            {
                debugString += "LOCAL_COMMS&";
            }

            if (enableTutorial)
            {
                debugString += "RESET_TUTORIAL&";
            }

            if (soloScene)
            {
                debugString += "LOS=0&";
            }

            if (builderInWorld)
            {
                debugString += "ENABLE_BUILDER_IN_WORLD&";
            }

            if (disableAssetBundles)
            {
                debugString += "DISABLE_ASSET_BUNDLES&DISABLE_WEARABLE_ASSET_BUNDLES&";
            }

            if (enableDebugMode)
            {
                debugString += "DEBUG_MODE&";
            }
            
            if (!string.IsNullOrEmpty(realm))
            {
                debugString += $"realm={realm}&";
            }


            string debugPanelString = "";

             if (!string.IsNullOrEmpty(kernelVersion))
             {
                 debugString += $"kernel-version={kernelVersion}&";
             }

             if (forceLocalComms)
             {
                 debugString += "LOCAL_COMMS&";
             }
            
             if (enableTutorial)
             {
                 debugString += "RESET_TUTORIAL&";
             }

             if (parcelRadiusToLoad != 4)
             {
                 debugString += $"LOS={parcelRadiusToLoad}&";
             }
            if (disableAssetBundles)
            {

            debugString += "DISABLE_ASSET_BUNDLES&DISABLE_WEARABLE_ASSET_BUNDLES&";
            RendereableAssetLoadHelper.defaultLoadingType = RendereableAssetLoadHelper.LoadingType.GLTF_ONLY;

                debugPanelString = "ENGINE_DEBUG_PANEL&";
            }
            else if (debugPanelMode == DebugPanel.Scene)
            {
                debugPanelString = "SCENE_DEBUG_PANEL&";
            }

            if (webSocketSSL)
            {
                Debug.Log(
                    "[REMINDER] To be able to connect with SSL you should start Chrome with the --ignore-certificate-errors argument specified (or enabling the following option: chrome://flags/#allow-insecure-localhost). In Firefox set the configuration option `network.websocket.allowInsecureFromHTTPS` to true, then use the ws:// rather than the wss:// address.");
            }

             if (!string.IsNullOrEmpty(realm))
             {
                 debugString += $"realm={realm}&";
             }


             if (!webSocketSSL)
             {
                 if (baseUrl.Contains("play.decentraland.org"))
                 {
                     Debug.LogError("play.decentraland.org only works with WebSocket SSL, please change the base URL to play.decentraland.zone");
                     QuitGame();
                     return;
                 }
             }
             else
             {
                 Debug.Log("[REMINDER] To be able to connect with SSL you should start Chrome with the --ignore-certificate-errors argument specified (or enabling the following option: chrome://flags/#allow-insecure-localhost). In Firefox set the configuration option `network.websocket.allowInsecureFromHTTPS` to true, then use the ws:// rather than the wss:// address.");                
             }


            webViewURL = $"{baseUrl}{debugString}{debugPanelString}position={startInCoords.x}%2C{startInCoords.y}&ws={DataStore.i.wsCommunication.url}";
            urlInput.text = webViewURL;
            Debug.Log(webViewURL);
            var canvas = GameObject.Find("Canvas");
#if UNITY_ANDROID && !UNITY_EDITOR
//don't have a method of using external browser on quest2.
openInternalBrowser = true;
#endif

            if (openInternalBrowser)
            {
                browserMessage.text = "Browser Loading";
                //DontDestroyOnLoad(canvas);
                WebViewOptions opt = new WebViewOptions();
#if ( UNITY_EDITOR || UNITY_STANDALONE)
                opt.preferredPlugins  = new WebPluginType[] { WebPluginType.AndroidGecko, WebPluginType.iOS, WebPluginType.Windows, WebPluginType.UniversalWindowsPlatform };
#elif UNITY_ANDROID
            opt.preferredPlugins  = new WebPluginType[] { WebPluginType.AndroidGecko};
#endif

               
                DCLWebview.InitialUrl = webViewURL;
                
                //DCLWebview.WebView.Reload();
                if (DCLWebview.WebView!= null && DCLWebview.WebView.IsInitialized) { 
                   
                   
                    DCLWebview.gameObject.SetActive((true));
                    keyboardDCL.gameObject.SetActive((true));
                    Debug.Log($"main webview loading {webViewURL}");
                    DCLWebview.WebView.LoadUrl(webViewURL);
                    DCLWebview.WebView.LoadProgressChanged += ( sender,  args) => {  
                        browserMessage.transform.parent.gameObject.SetActive((false));
                        Debug.Log($"WebView Status LoadProgressChanged {args.Type.ToString()}, {sender.ToString()}");
                    };
                    DCLWebview.WebView.PageLoadFailed += ( sender,  args) => { Debug.Log($"WebView Status PageLoadFailed {args.ToString()}, {sender.ToString()}"); };
                    DCLWebview.WebView.CloseRequested += ( sender,  args) => { Debug.Log($"WebView Status CloseRequested {args.ToString()}, {sender.ToString()}"); };}
                else
                {
                    DCLWebview.Initialized += (sender, eventArgs) =>
                    {
                        
                        
                        DCLWebview.gameObject.SetActive((true));
                        keyboardDCL.gameObject.SetActive((true));
                        Debug.Log($"main webview loading {webViewURL}");
                        DCLWebview.WebView.LoadUrl(webViewURL);
                        DCLWebview.WebView.LoadProgressChanged += ( sender,  args) => {  
                            browserMessage.transform.parent.gameObject.SetActive((false));
                            Debug.Log($"WebView Status LoadProgressChanged {args.Type.ToString()}, {sender.ToString()}"); 
                        };
                        DCLWebview.WebView.PageLoadFailed += ( sender,  args) => { Debug.Log($"WebView Status PageLoadFailed {args.ToString()}, {sender.ToString()}"); };
                        DCLWebview.WebView.CloseRequested += ( sender,  args) => { Debug.Log($"WebView Status CloseRequested {args.ToString()}, {sender.ToString()}"); };
                    };
                }


                DCLWebview.transform.SetParent(canvas.transform, false);
                DCLWebview.Resolution = 450;


                // DCLWebview.RemoteDebuggingEnabled = false;
                // DCLWebview.LogConsoleMessages = false;
                DCLWebview.NativeOnScreenKeyboardEnabled = false;
                DCLWebview.Native2DModeEnabled = false;

                urlInput.keyboardType = TouchScreenKeyboardType.URL;
                urlInput.contentType = TMP_InputField.ContentType.Alphanumeric;


                Debug.Log("Created WebView objects");
                //_positionPrefabs();
                Debug.Log("finished positioning webview objects");
            }
            else
            {
                browserMessage.text = "Use External Browser";
                Application.OpenURL(webViewURL);
            }
            //#endif

        }
        private void _positionPrefabs() {
        #if (UNITY_ANDROID || UNITY_STANDALONE || UNITY_EDITOR)
            var rectTransform = DCLWebview.transform as RectTransform;
           
            
            //rectTransform.anchoredPosition3D = Vector3.zero;
            rectTransform.offsetMin = new Vector2(-0.191f, -0.25f);
            rectTransform.offsetMax = new Vector2(-0.809f, -0.75f);
            rectTransform.pivot = new Vector2(0.5f, 1);
            //rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 520/150);
            //rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 520/150);

            rectTransform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        
            // var keyboardTransform = _keyboard.transform as RectTransform;
            // keyboardTransform.anchoredPosition3D = Vector3.zero;
            // keyboardTransform.offsetMin = new Vector2(0.5f, -1.8f);
            // keyboardTransform.offsetMax = new Vector2(0.5f, -0.3f);
            // keyboardTransform.pivot = new Vector2(0.5f, 0);
            // keyboardTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 690/150);
            // keyboardTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 162/150);
#endif
        }

// <<<<<<< HEAD
        
        public  void HideWebViewScreens()
        {
            Debug.Log("WebView Connected, hiding web browsers.");
            // _canvasWebViewPrefab.Visible = false;
             //DCLWebview.transform.localPosition += new Vector3(0, 1, 0);
            //_keyboard.WebViewPrefab.transform.localPosition -= new Vector3(0, 10, 0);
            startMenu.SetActive((false));
              keyboardOptions.gameObject.SetActive(false);
              keyboardDCL.gameObject.SetActive((false));
              optionsWeview.gameObject.SetActive(false);
              urlInput.gameObject.SetActive(false);
             reload.gameObject.SetActive((false));
             swapTabs.gameObject.SetActive((false));
              DCLWebview.gameObject.SetActive(false);
            

        }
        public void ShowWebviewScreen()
        {
            popupMessageObj.SetActive(true);
            popupMessage.text = "Network Communication Lost.\r\nRestart Application.\r\n Reduced Loading Radius Recommended In This Area";
            ReloadPage();
            
            //DCLWebview.gameObject.SetActive(true);
            //keyboardDCL.gameObject.SetActive((true));
            //urlInput.gameObject.SetActive(false);
            //reload.gameObject.SetActive((true));


        }
        public void SwapBrowserTabs()
        {
            if (isMainTab)
            {
                DCLWebview.gameObject.SetActive((false));
                keyboardDCL.gameObject.SetActive(false);
                optionsWeview.gameObject.SetActive((true));
                keyboardOptions.gameObject.SetActive(true);
                isMainTab = false;
            }
            else
            {
                DCLWebview.gameObject.SetActive((true));
                keyboardDCL.gameObject.SetActive(true);
                optionsWeview.gameObject.SetActive((false));
                keyboardOptions.gameObject.SetActive(false);
                isMainTab = true;
            }
                
        }


        public void ReloadPage()
        {
        //TODO: ensure websocket is restarted and listening, set start location to current parcel, ensure startMenu is open, reload url. Use to correct dropped connections in future.
            //Set
        
            //if(_canvasWebViewPrefab!=null) _canvasWebViewPrefab.Destroy();
            //if(_keyboard!= null)  Destroy(_keyboard.gameObject);
            if(openInternalBrowser)
                DCLWebview.WebView.Reload();
            else
                OpenWebBrowser();

        }
        public void PauseWebview()
        {
            //DCLWebview.WebView.Dispose();
        }

       
        public void ToggleUseInternalBrowser()
        {

            openInternalBrowser = useInternalBrowser.isOn;
            WebInterface.openURLInternal = openInternalBrowser;
            // if (useInternalBrowser.isOn)
            // {
            //     DCLWebview.gameObject.SetActive(true);
            //     keyboardDCL.gameObject.SetActive(true);
            //     optionsWeview.gameObject.SetActive(false);
            //     keyboardOptions.gameObject.SetActive(false);
            //     OpenWebBrowser();
            // }
            // else
            // {
            //     if (DCLWebview != null && DCLWebview.WebView != null)
            //     {
            //         DCLWebview.WebView.LoadUrl("https://www.google.com/");
            //         //DCLWebview.WebView.StopLoad();
            //         //DCLWebview.WebView.Dispose();
            //         DCLWebview.gameObject.SetActive((false));
            //     }
            //     DCLWebview.gameObject.SetActive(false);
            //     keyboardDCL.gameObject.SetActive(false);
            //     optionsWeview.gameObject.SetActive(false);
            //     keyboardOptions.gameObject.SetActive(false);
            //     OpenWebBrowser();
            // }
            
        }
        

        private void OnDestroy() { DataStore.i.wsCommunication.communicationReady.OnChange -= OnCommunicationReadyChangedValue; }
       

        private void QuitGame()
        {
#if UNITY_EDITOR
            // Application.Quit() does not work in the editor so
            // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        
   
    }
    
}