using Dot.Core.Asset;
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
            DontDestroyOnLoad(gameObject);
            
            timerMgr = TimerManager.GetInstance();
            assetLoader = AssetLoader.GetInstance();
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            timerMgr.DoUpdate(deltaTime);
            assetLoader.DoUpdate();
        }
    }
}
