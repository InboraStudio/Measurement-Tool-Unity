using UnityEngine;
using UnityEditor;

namespace PhotoLabs.Spectator
{
#if UNITY_EDITOR
    public class MeasurementToolSetup : MonoBehaviour
    {
        [MenuItem("PhotoLabs/Spectator/Add Measurement Tool")]
        public static void AddMeasurementToolToScene()
        {
            // Create a parent container for the tool
            GameObject toolContainer = new GameObject("MeasurementTool");
            
            // Add the manager component
            MeasurementToolManager manager = toolContainer.AddComponent<MeasurementToolManager>();
            
            // Set up a basic UI canvas
            GameObject canvas = new GameObject("MeasurementUI_Canvas");
            canvas.transform.SetParent(toolContainer.transform);
            
            Canvas canvasComponent = canvas.AddComponent<Canvas>();
            canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvas.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            
            // Create a basic UI panel
            GameObject panel = new GameObject("Panel");
            panel.transform.SetParent(canvas.transform);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0, 1);
            panelRect.anchorMax = new Vector2(0, 1);
            panelRect.pivot = new Vector2(0, 1);
            panelRect.anchoredPosition = new Vector2(10, -10);
            panelRect.sizeDelta = new Vector2(200, 100);
            
            UnityEngine.UI.Image panelImage = panel.AddComponent<UnityEngine.UI.Image>();
            panelImage.color = new Color(0, 0, 0, 0.7f);
            
            // Create distance text
            GameObject distanceTextObj = new GameObject("DistanceText");
            distanceTextObj.transform.SetParent(panel.transform);
            RectTransform distanceRect = distanceTextObj.AddComponent<RectTransform>();
            distanceRect.anchorMin = new Vector2(0, 1);
            distanceRect.anchorMax = new Vector2(1, 1);
            distanceRect.pivot = new Vector2(0.5f, 1);
            distanceRect.sizeDelta = new Vector2(0, 30);
            distanceRect.anchoredPosition = new Vector2(0, -15);
            
            TMPro.TextMeshProUGUI distanceText = distanceTextObj.AddComponent<TMPro.TextMeshProUGUI>();
            distanceText.text = "Distance: 0 units";
            distanceText.fontSize = 14;
            distanceText.alignment = TMPro.TextAlignmentOptions.Center;
            distanceText.color = Color.white;
            
            // Create angle text
            GameObject angleTextObj = new GameObject("AngleText");
            angleTextObj.transform.SetParent(panel.transform);
            RectTransform angleRect = angleTextObj.AddComponent<RectTransform>();
            angleRect.anchorMin = new Vector2(0, 1);
            angleRect.anchorMax = new Vector2(1, 1);
            angleRect.pivot = new Vector2(0.5f, 1);
            angleRect.sizeDelta = new Vector2(0, 30);
            angleRect.anchoredPosition = new Vector2(0, -45);
            
            TMPro.TextMeshProUGUI angleText = angleTextObj.AddComponent<TMPro.TextMeshProUGUI>();
            angleText.text = "Angle: 0°";
            angleText.fontSize = 14;
            angleText.alignment = TMPro.TextAlignmentOptions.Center;
            angleText.color = Color.white;
            
            // Add UI controller
            MeasurementToolUI uiController = panel.AddComponent<MeasurementToolUI>();
            
            // Set references via reflection
            uiController.GetType().GetField("distanceText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(uiController, distanceText);
            
            uiController.GetType().GetField("angleText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(uiController, angleText);
            
            uiController.GetType().GetField("uiPanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(uiController, panelRect);
            
            // Assign canvas to manager
            manager.GetType().GetField("measurementCanvas", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(manager, canvasComponent);
            
            // Create a prefab of this setup
            #if UNITY_EDITOR
            // Save the prefab
            if (!AssetDatabase.IsValidFolder("Assets/Spectator/MeasurementTool/Prefabs"))
            {
                AssetDatabase.CreateFolder("Assets/Spectator/MeasurementTool", "Prefabs");
            }
            
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(toolContainer, "Assets/Spectator/MeasurementTool/Prefabs/MeasurementTool.prefab");
            EditorUtility.DisplayDialog("Measurement Tool Added", 
                "The Measurement Tool has been added to your scene.\n\n" +
                "Press 'M' to toggle it on/off.\n\n" +
                "A prefab has also been created at:\n" +
                "Assets/Spectator/MeasurementTool/Prefabs/MeasurementTool.prefab", 
                "OK");
            #endif
            
            // Select the created object
            Selection.activeGameObject = toolContainer;
        }
        
        [MenuItem("PhotoLabs/Spectator/Add Measurement Tool To Mouse Tracker")]
        public static void AddMeasurementToolToMouseTracker()
        {
            // Find the MouseTracker in the scene
            GameObject mouseTrackerObj = GameObject.Find("MouseTracker");
            
            if (mouseTrackerObj == null)
            {
                EditorUtility.DisplayDialog("Mouse Tracker Not Found", 
                    "Could not find a MouseTracker object in the current scene.\n\n" +
                    "Please make sure the MouseTracker is in your scene and try again.", 
                    "OK");
                return;
            }
            
            // Add the MeasurementToolManager to the MouseTracker
            MeasurementToolManager manager = mouseTrackerObj.AddComponent<MeasurementToolManager>();
            
            // Create UI for the measurement tool
            GameObject uiObj = new GameObject("MeasurementUI");
            
            // Try to find an existing Canvas in the MouseTracker
            Transform mouseTrackerCanvas = mouseTrackerObj.transform.Find("Canvas");
            if (mouseTrackerCanvas != null)
            {
                uiObj.transform.SetParent(mouseTrackerCanvas);
                
                // Assign the canvas to the manager
                Canvas canvasComponent = mouseTrackerCanvas.GetComponent<Canvas>();
                if (canvasComponent != null)
                {
                    manager.GetType().GetField("measurementCanvas", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        ?.SetValue(manager, canvasComponent);
                }
            }
            else
            {
                // Create a new canvas if not found
                GameObject canvas = new GameObject("MeasurementUI_Canvas");
                canvas.transform.SetParent(mouseTrackerObj.transform);
                
                Canvas canvasComponent = canvas.AddComponent<Canvas>();
                canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.AddComponent<UnityEngine.UI.CanvasScaler>();
                canvas.AddComponent<UnityEngine.UI.GraphicRaycaster>();
                
                uiObj.transform.SetParent(canvas.transform);
                
                // Assign canvas to manager
                manager.GetType().GetField("measurementCanvas", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(manager, canvasComponent);
            }
            
            // Set up the UI panel
            RectTransform uiRect = uiObj.AddComponent<RectTransform>();
            uiRect.anchorMin = new Vector2(0, 1);
            uiRect.anchorMax = new Vector2(0, 1);
            uiRect.pivot = new Vector2(0, 1);
            uiRect.anchoredPosition = new Vector2(10, -10);
            uiRect.sizeDelta = new Vector2(200, 100);
            
            // Add panel background
            GameObject panel = new GameObject("Panel");
            panel.transform.SetParent(uiObj.transform);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.sizeDelta = Vector2.zero;
            
            UnityEngine.UI.Image panelImage = panel.AddComponent<UnityEngine.UI.Image>();
            panelImage.color = new Color(0, 0, 0, 0.7f);
            
            // Create distance text
            GameObject distanceTextObj = new GameObject("DistanceText");
            distanceTextObj.transform.SetParent(panel.transform);
            RectTransform distanceRect = distanceTextObj.AddComponent<RectTransform>();
            distanceRect.anchorMin = new Vector2(0, 1);
            distanceRect.anchorMax = new Vector2(1, 1);
            distanceRect.pivot = new Vector2(0.5f, 1);
            distanceRect.sizeDelta = new Vector2(0, 30);
            distanceRect.anchoredPosition = new Vector2(0, -15);
            
            TMPro.TextMeshProUGUI distanceText = distanceTextObj.AddComponent<TMPro.TextMeshProUGUI>();
            distanceText.text = "Distance: 0 units";
            distanceText.fontSize = 14;
            distanceText.alignment = TMPro.TextAlignmentOptions.Center;
            distanceText.color = Color.white;
            
            // Create angle text
            GameObject angleTextObj = new GameObject("AngleText");
            angleTextObj.transform.SetParent(panel.transform);
            RectTransform angleRect = angleTextObj.AddComponent<RectTransform>();
            angleRect.anchorMin = new Vector2(0, 1);
            angleRect.anchorMax = new Vector2(1, 1);
            angleRect.pivot = new Vector2(0.5f, 1);
            angleRect.sizeDelta = new Vector2(0, 30);
            angleRect.anchoredPosition = new Vector2(0, -45);
            
            TMPro.TextMeshProUGUI angleText = angleTextObj.AddComponent<TMPro.TextMeshProUGUI>();
            angleText.text = "Angle: 0°";
            angleText.fontSize = 14;
            angleText.alignment = TMPro.TextAlignmentOptions.Center;
            angleText.color = Color.white;
            
            // Add UI controller
            MeasurementToolUI uiController = uiObj.AddComponent<MeasurementToolUI>();
            
            // Set references via reflection
            uiController.GetType().GetField("distanceText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(uiController, distanceText);
            
            uiController.GetType().GetField("angleText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(uiController, angleText);
            
            uiController.GetType().GetField("uiPanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(uiController, uiRect);
            
            // Assign the UI to the manager
            manager.GetType().GetField("uiPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(manager, uiObj);
            
            // Display confirmation
            EditorUtility.DisplayDialog("Measurement Tool Added", 
                "The Measurement Tool has been added to the MouseTracker.\n\n" +
                "Press 'M' to toggle it on/off.", 
                "OK");
            
            // Select the MouseTracker
            Selection.activeGameObject = mouseTrackerObj;
        }
    }
#endif
} 