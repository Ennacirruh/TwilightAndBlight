using UnityEngine;
namespace TwilightAndBlight
{
    public class GameManager : MonoBehaviour
    {

        private static GameManager instance;
        public static GameManager Instance { get { return instance; } }
        [SerializeField] private Canvas mainCanvas;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(this);
            }
        }

        public Canvas GetMainCanvas()
        {
            return mainCanvas;
        }

    }
}