using UnityEngine;
using TMPro;

namespace PhotoLabs.Spectator
{
    public class MeasurementToolUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI distanceText;
        [SerializeField] private TextMeshProUGUI angleText;
        [SerializeField] private RectTransform uiPanel;
        
        private Canvas canvas;
        private Camera mainCamera;
        
        private void Awake()
        {
            canvas = GetComponentInParent<Canvas>();
            mainCamera = Camera.main;
            
            // Ensure UI is initially hidden
            gameObject.SetActive(false);
        }
        
        private void Update()
        {
            // Follow the cursor position with UI panel
            if (uiPanel != null && canvas != null && canvas.renderMode != RenderMode.WorldSpace)
            {
                Vector2 mousePos = Input.mousePosition;
                
                // Ensure the panel stays on screen
                float panelWidth = uiPanel.rect.width;
                float panelHeight = uiPanel.rect.height;
                float offset = 20f;
                
                // Adjust position if it would go off-screen
                if (mousePos.x + panelWidth + offset > Screen.width)
                {
                    mousePos.x -= panelWidth + offset;
                }
                else
                {
                    mousePos.x += offset;
                }
                
                if (mousePos.y - panelHeight - offset < 0)
                {
                    mousePos.y += offset;
                }
                else
                {
                    mousePos.y -= offset;
                }
                
                uiPanel.position = mousePos;
            }
        }
        
        public void SetDistanceText(string text)
        {
            if (distanceText != null)
            {
                distanceText.text = text;
                distanceText.gameObject.SetActive(!string.IsNullOrEmpty(text));
            }
        }
        
        public void SetAngleText(string text)
        {
            if (angleText != null)
            {
                angleText.text = text;
                angleText.gameObject.SetActive(!string.IsNullOrEmpty(text));
            }
        }
    }
} 