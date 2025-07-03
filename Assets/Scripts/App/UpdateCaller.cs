
using UnityEngine;


namespace SpriteMapper
{
    /// <summary>
    /// <br/>   Runs all subscribed callback methods each MonoBehaviour Update().
    /// <br/>   Code from: <see href="https://discussions.unity.com/t/how-to-call-update-from-a-class-thats-not-inheriting-from-monobehaviour/652560/4"/>
    /// </summary>
    public class UpdateCaller : MonoBehaviour
    {
        private static UpdateCaller instance;

        private System.Action updateCallback;


        private void Update() { updateCallback(); }


        public static void SubscribeUpdateCallback(System.Action updateMethod)
        {
            if (instance == null)
            {
                GameObject gameObject = new("[Update Caller]");
                instance = gameObject.AddComponent<UpdateCaller>();
                
                DontDestroyOnLoad(gameObject);
            }

            instance.updateCallback += updateMethod;
        }

        public static void UnsubscribeUpdateCallback(System.Action updateMethod)
        {
            if (instance != null) { instance.updateCallback -= updateMethod; }
        }
    }
}
