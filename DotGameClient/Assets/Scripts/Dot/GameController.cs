using Dot.Core.Asset;
using Dot.Core.Loader;
using Dot.Core.Timer;
using Dot.Core.Util;
using UnityEngine;

namespace Dot
{
    public class GameController : MonoBehaviour
    {
        private static GameController gameController = null;
        public static void StartUp()
        {
            if(gameController == null)
            {
                DontDestroyHandler.CreateComponent<GameController>();
            }
        }

        public static GameController GetController()
        {
            return gameController;
        }

        private TimerManager timerMgr = null;
        private AssetLoader assetLoader = null;

        private void Awake()
        {
            if(gameController!=null)
            {
                Destroy(this);
                return;
            }
            gameController = this;
            
            timerMgr = TimerManager.GetInstance();
            assetLoader = AssetLoader.GetInstance();
        }

        private float delayTime = 0;
        private void Update()
        {
            float deltaTime = Time.deltaTime;

            timerMgr.DoUpdate(deltaTime);
            assetLoader.DoUpdate();
            delayTime += deltaTime;
            if(delayTime>=1.0f)
            {
                AssetManager.GetInstance().DoUpdate(deltaTime);
                delayTime = 0.0f;
            }
        }
    }
}
