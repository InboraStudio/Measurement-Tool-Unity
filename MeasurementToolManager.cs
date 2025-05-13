using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PhotoLabs.Spectator
{
    /// <summary>
    /// Manager class for the distance and angle measurement tool.
    /// Add this to a GameObject in your scene to enable measurements.
    /// </summary>
    public class MeasurementToolManager : MonoBehaviour
    {
        [Header("Tool Settings")]
        [SerializeField] private KeyCode toggleKey = KeyCode.M;
        [SerializeField] private Color measurementColor = Color.yellow;
        [SerializeField] private float lineWidth = 2f;
        [SerializeField] private LayerMask measurableLayers = ~0; // All layers by default
        
        [Header("UI References")]
        [SerializeField] private Canvas measurementCanvas;
        [SerializeField] private GameObject uiPrefab;
        
        [Header("Point Markers")]
        [SerializeField] private GameObject pointMarkerPrefab;
        [SerializeField] private GameObject angleMarkerPrefab;
        
        private MeasurementTool activeTool;
        private GameObject uiInstance;
        private bool isToolActive = false;
        
        private void Start()
        {
            // Create UI if not already set
            if (measurementCanvas == null)
            {
                CreateDefaultCanvas();
            }
            
            // Initialize the tool as inactive
            isToolActive = false;
        }
        
        private void Update()
        {
            // Toggle the measurement tool on/off
            if (Input.GetKeyDown(toggleKey))
            {
                ToggleMeasurementTool();
            }
        }
        
        private void CreateDefaultCanvas()
        {
            // Create a new Canvas for UI
            GameObject canvasObject = new GameObject("MeasurementToolCanvas");
            measurementCanvas = canvasObject.AddComponent<Canvas>();
            measurementCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            // Add required components
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
            
            // Position the canvas in the hierarchy
            canvasObject.transform.SetParent(transform);
        }
        
        public void ToggleMeasurementTool()
        {
            isToolActive = !isToolActive;
            
            if (isToolActive)
            {
                EnableMeasurementTool();
            }
            else
            {
                DisableMeasurementTool();
            }
        }
        
        private void EnableMeasurementTool()
        {
            // Create tool if it doesn't exist
            if (activeTool == null)
            {
                GameObject toolObject = new GameObject("MeasurementTool");
                toolObject.transform.SetParent(transform);
                activeTool = toolObject.AddComponent<MeasurementTool>();
                
                // Configure the tool
                LineRenderer lineRenderer = toolObject.AddComponent<LineRenderer>();
                lineRenderer.startWidth = lineWidth;
                lineRenderer.endWidth = lineWidth;
                lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
                lineRenderer.startColor = measurementColor;
                lineRenderer.endColor = measurementColor;
                
                // Assign the line renderer to the tool
                activeTool.GetType().GetField("lineRenderer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(activeTool, lineRenderer);
                
                // Set up the UI
                if (uiPrefab != null)
                {
                    uiInstance = Instantiate(uiPrefab, measurementCanvas.transform);
                    activeTool.GetType().GetField("measurementUI", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        ?.SetValue(activeTool, uiInstance);
                    
                    // Try to find and assign text components
                    TextMeshProUGUI[] texts = uiInstance.GetComponentsInChildren<TextMeshProUGUI>();
                    if (texts.Length > 0)
                    {
                        if (texts.Length > 0)
                        {
                            activeTool.GetType().GetField("distanceText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                                ?.SetValue(activeTool, texts[0]);
                        }
                        if (texts.Length > 1)
                        {
                            activeTool.GetType().GetField("angleText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                                ?.SetValue(activeTool, texts[1]);
                        }
                    }
                }
                else
                {
                    // Create a simple UI if no prefab is provided
                    CreateSimpleUI();
                }
                
                // Assign prefabs if available
                activeTool.GetType().GetField("pointPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(activeTool, pointMarkerPrefab);
                
                activeTool.GetType().GetField("anglePrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(activeTool, angleMarkerPrefab);
                
                // Set measurable layers
                activeTool.GetType().GetField("measurableLayers", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(activeTool, measurableLayers);
                
                // Set color
                activeTool.GetType().GetField("lineColor", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(activeTool, measurementColor);
                
                // Set line width
                activeTool.GetType().GetField("lineWidth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(activeTool, lineWidth);
            }
            
            // Enable the tool
            activeTool.gameObject.SetActive(true);
            
            Debug.Log("Measurement Tool activated. Click to measure distance. Hold Shift while dragging to measure angles.");
        }
        
        private void DisableMeasurementTool()
        {
            if (activeTool != null)
            {
                // Clear any active measurements
                activeTool.ClearMeasurement();
                
                // Disable the tool
                activeTool.gameObject.SetActive(false);
            }
            
            Debug.Log("Measurement Tool deactivated.");
        }
        
        private void CreateSimpleUI()
        {
            // Create a simple UI panel
            GameObject uiObject = new GameObject("MeasurementUI");
            uiObject.transform.SetParent(measurementCanvas.transform);
            
            // Add a panel background
            GameObject panel = new GameObject("Panel");
            panel.transform.SetParent(uiObject.transform);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0, 1);
            panelRect.anchorMax = new Vector2(0, 1);
            panelRect.pivot = new Vector2(0, 1);
            panelRect.sizeDelta = new Vector2(200, 100);
            
            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 0.7f);
            
            // Add distance text
            GameObject distanceTextObj = new GameObject("DistanceText");
            distanceTextObj.transform.SetParent(panel.transform);
            RectTransform distanceRect = distanceTextObj.AddComponent<RectTransform>();
            distanceRect.anchorMin = new Vector2(0, 1);
            distanceRect.anchorMax = new Vector2(1, 1);
            distanceRect.pivot = new Vector2(0.5f, 1);
            distanceRect.sizeDelta = new Vector2(0, 30);
            distanceRect.anchoredPosition = new Vector2(0, -10);
            
            TextMeshProUGUI distText = distanceTextObj.AddComponent<TextMeshProUGUI>();
            distText.text = "Distance: 0 units";
            distText.fontSize = 14;
            distText.alignment = TextAlignmentOptions.Center;
            distText.color = Color.white;
            
            // Add angle text
            GameObject angleTextObj = new GameObject("AngleText");
            angleTextObj.transform.SetParent(panel.transform);
            RectTransform angleRect = angleTextObj.AddComponent<RectTransform>();
            angleRect.anchorMin = new Vector2(0, 1);
            angleRect.anchorMax = new Vector2(1, 1);
            angleRect.pivot = new Vector2(0.5f, 1);
            angleRect.sizeDelta = new Vector2(0, 30);
            angleRect.anchoredPosition = new Vector2(0, -40);
            
            TextMeshProUGUI angText = angleTextObj.AddComponent<TextMeshProUGUI>();
            angText.text = "Angle: 0°";
            angText.fontSize = 14;
            angText.alignment = TextAlignmentOptions.Center;
            angText.color = Color.white;
            
            // Add the UI follow script
            MeasurementToolUI uiScript = uiObject.AddComponent<MeasurementToolUI>();
            
            // Set UI references
            uiScript.GetType().GetField("distanceText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(uiScript, distText);
            
            uiScript.GetType().GetField("angleText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(uiScript, angText);
            
            uiScript.GetType().GetField("uiPanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(uiScript, panelRect);
            
            // Assign to the tool
            uiInstance = uiObject;
            activeTool.GetType().GetField("measurementUI", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(activeTool, uiInstance);
            
            activeTool.GetType().GetField("distanceText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(activeTool, distText);
            
            activeTool.GetType().GetField("angleText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(activeTool, angText);
        }
    }
} 