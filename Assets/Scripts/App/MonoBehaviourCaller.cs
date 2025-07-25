
using UnityEngine;


namespace SpriteMapper
{
    /// <summary>
    /// <br/>   Runs callback methods subscribed to MonoBehaviour events.
    /// <br/>   Code help from: <see href="https://discussions.unity.com/t/how-to-call-update-from-a-class-thats-not-inheriting-from-monobehaviour/652560/4"/>
    /// </summary>
    public class MonoBehaviourCaller : MonoBehaviour
    {
        public static MonoBehaviourCaller Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject gameObject = new("[MonoBehaviour Caller]");
                    instance = gameObject.AddComponent<MonoBehaviourCaller>();

                    DontDestroyOnLoad(gameObject);
                }

                return instance;
            }
        }

        public System.Action UpdateCallback { get; set; }
        public System.Action FixedUpdateCallback { get; set; }


        private static MonoBehaviourCaller instance = null;


        private void Update() { UpdateCallback?.Invoke(); }
        private void FixedUpdate() { FixedUpdateCallback?.Invoke(); }
    }
}
