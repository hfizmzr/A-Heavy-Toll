using UnityEngine;

namespace AHeavyToll.Managers
{
    public class CursorManager : MonoBehaviour
    {
        public static CursorManager Instance { get; private set; }

        private int _requestCount = 0;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void RequestCursor()
        {
            _requestCount++;
            UpdateCursorState();
        }

        public void ReleaseCursor()
        {
            _requestCount = Mathf.Max(0, _requestCount - 1);
            UpdateCursorState();
        }

        private void UpdateCursorState()
        {
            if (_requestCount > 0)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}
