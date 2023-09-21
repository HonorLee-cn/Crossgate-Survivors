using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using Controller;
using DG.Tweening;
using Game.UI;
using GameData;
using Reference;
using UnityEngine;
using UnityEngine.Networking;
using Util;
using Debug = UnityEngine.Debug;

namespace Scene
{
    // 游戏场景
    public class GameScene:MonoBehaviour
    {
        [SerializeField,Header("UI引用")] private Reference.UIRef uiRef;
        [SerializeField,Header("预制体引用")] private Reference.PrefebRef prefebRef;
        [SerializeField,Header("Loading")] private  LoadingScreen loadingScreen;
        private int screenH;
        
        // 版本更新检测
        private string VersionUrl = "https://honorlee.me/game/cgs/version.html";
        private float version = 0.15f;
        private void Start()
        {
            //初始化全局引用
            GlobalReference.UI = uiRef;
            GlobalReference.Prefeb = prefebRef;
            // 固定分辨率
            Screen.SetResolution(1366,768,false);
            
            //调整摄像机
            GlobalReference.UI.PlaygroundCamera.orthographicSize = Screen.height / 2;
            GlobalReference.UI.PlaygroundCamera.transparencySortMode = TransparencySortMode.CustomAxis;
            GlobalReference.UI.PlaygroundCamera.transparencySortAxis = new Vector3(0, 1, 0);

            //载入动画
            loadingScreen.Logo.DOFade(1, 0.5f).onComplete=(() =>
            {
                loadingScreen.Loading.DOFade(0.5f, 0.5f).onComplete = (() =>
                {
                    initGame();
                });
            });
            
        }

        public void initGame()
        {

            string path = "";
            
#if PLATFORM_STANDALONE_OSX
            path = Application.streamingAssetsPath;
            path = Path.Combine(path, "bin");
#endif
#if PLATFORM_STANDALONE_Windows
            string executableDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            path = executableDirectory + "/bin";
#endif
#if UNITY_EDITOR
            path = null;
#endif
            
            //初始化CGTool
            try
            {
                CGTool.CGTool.Init(path);
            }
            catch (Exception e)
            {
                loadingScreen.Logo.gameObject.SetActive(false);
                loadingScreen.Loading.gameObject.SetActive(false);
                loadingScreen.ErrorObject.SetActive(true);
                loadingScreen.ErrorText.text = path;
                return;
            }


            loadingScreen.Loading.gameObject.SetActive(false);
            loadingScreen.Logo.DOFade(0, 1f).onComplete=(() =>
            {
                loadingScreen.gameObject.SetActive(false);
                GlobalReference.UI.MainScreen.gameObject.SetActive(true);
                GlobalReference.UI.MainScreen.CurrentVersion.text = "Ver. " + version;
                StartCoroutine(UpdateCheck());
            });
        }

        private void Update()
        {
            GameController.Update();
        }

        public void StartGame()
        {
            GameController.StartGame();
        }
        private void OnGUI()
        {
            //分辨率适配
            if (Event.current.type == EventType.Repaint)
            {
                if (screenH != Screen.height)
                {
                    screenH = Screen.height;
                    GlobalReference.UI.PlaygroundCamera.orthographicSize = Screen.height/2;
                    
                }
                
            }
        }
        
        //获取更新版本号
        IEnumerator UpdateCheck()
        {
            GlobalReference.UI.MainScreen.UpdateText.gameObject.SetActive(false);
            using (UnityWebRequest webRequest = UnityWebRequest.Get(VersionUrl)){
                yield return webRequest.SendWebRequest();
                if (webRequest.isNetworkError || webRequest.isHttpError)
                {
                    Debug.LogError("网络请求错误: " + webRequest.error);
                }
                else
                {
                    string textContent = webRequest.downloadHandler.text;
                    float newVersion = float.Parse(textContent);
                    if (newVersion > version)
                    {
                        Debug.Log("有新版本");
                        GlobalReference.UI.MainScreen.UpdateText.gameObject.SetActive(true);
                        GlobalReference.UI.MainScreen.UpdateText.text =
                            "发现新版本(Ver." + newVersion + ")\n请进QQ群 131034182 获取最新版本";
                    }
                    else
                    {
                        GlobalReference.UI.MainScreen.UpdateText.gameObject.SetActive(true);
                        GlobalReference.UI.MainScreen.UpdateText.text =
                            "当前已是最新版本\n欢迎请进QQ群 131034182 讨论研究";
                    }
                }
            }
        }
    }
}