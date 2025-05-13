using UnityEngine;
using UnityEditor;

namespace PhotoLabs.Spectator
{
#if UNITY_EDITOR
    [CustomEditor(typeof(MeasurementToolManager))]
    public class MeasurementToolManagerEditor : Editor
    {
        private SerializedProperty toggleKeyProp;
        private SerializedProperty measurementColorProp;
        private SerializedProperty lineWidthProp;
        private SerializedProperty measurableLayersProp;
        private SerializedProperty measurementCanvasProp;
        private SerializedProperty uiPrefabProp;
        private SerializedProperty pointMarkerPrefabProp;
        private SerializedProperty angleMarkerPrefabProp;
        
        private bool showHelp = false;
        
        private void OnEnable()
        {
            toggleKeyProp = serializedObject.FindProperty("toggleKey");
            measurementColorProp = serializedObject.FindProperty("measurementColor");
            lineWidthProp = serializedObject.FindProperty("lineWidth");
            measurableLayersProp = serializedObject.FindProperty("measurableLayers");
            measurementCanvasProp = serializedObject.FindProperty("measurementCanvas");
            uiPrefabProp = serializedObject.FindProperty("uiPrefab");
            pointMarkerPrefabProp = serializedObject.FindProperty("pointMarkerPrefab");
            angleMarkerPrefabProp = serializedObject.FindProperty("angleMarkerPrefab");
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.Space();
            
            // Title and description
            GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
            titleStyle.fontSize = 14;
            titleStyle.alignment = TextAnchor.MiddleCenter;
            
            EditorGUILayout.LabelField("Measurement Tool Manager", titleStyle);
            
            EditorGUILayout.Space();
            
            // Help toggle
            showHelp = EditorGUILayout.Foldout(showHelp, "Show Help");
            if (showHelp)
            {
                EditorGUILayout.HelpBox(
                    "The Measurement Tool allows you to measure distances and angles in your scene.\n\n" +
                    "- Press the toggle key (default: M) to activate/deactivate the tool.\n" +
                    "- Click to measure distance between two points.\n" +
                    "- Hold Shift to measure angles.\n\n" +
                    "You can customize the appearance and behavior of the tool using the properties below.", 
                    MessageType.Info);
            }
            
            EditorGUILayout.Space();
            
            // Tool Settings
            EditorGUILayout.LabelField("Tool Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(toggleKeyProp, new GUIContent("Toggle Key", "The key used to toggle the measurement tool on/off."));
            EditorGUILayout.PropertyField(measurementColorProp, new GUIContent("Measurement Color", "The color of the measurement lines and markers."));
            EditorGUILayout.PropertyField(lineWidthProp, new GUIContent("Line Width", "The thickness of the measurement lines."));
            EditorGUILayout.PropertyField(measurableLayersProp, new GUIContent("Measurable Layers", "The layers that can be measured."));
            
            EditorGUILayout.Space();
            
            // UI References
            EditorGUILayout.LabelField("UI References", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(measurementCanvasProp, new GUIContent("Measurement Canvas", "The canvas used to display the measurement UI."));
            EditorGUILayout.PropertyField(uiPrefabProp, new GUIContent("UI Prefab", "The prefab used for the measurement UI."));
            
            // Create UI button
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Create Default UI", GUILayout.Width(150)))
            {
                CreateDefaultUI();
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            // Point Markers
            EditorGUILayout.LabelField("Point Markers", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(pointMarkerPrefabProp, new GUIContent("Point Marker Prefab", "The prefab used for marking measurement points."));
            EditorGUILayout.PropertyField(angleMarkerPrefabProp, new GUIContent("Angle Marker Prefab", "The prefab used for angle visualization."));
            
            // Create markers buttons
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Create Point Marker"))
            {
                PointMarkerCreator.CreatePointMarker();
                FindPointMarker();
            }
            if (GUILayout.Button("Create Angle Marker"))
            {
                PointMarkerCreator.CreateAngleMarker();
                FindAngleMarker();
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            // Quick action buttons
            EditorGUILayout.LabelField("Quick Actions", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Test Tool"))
            {
                TestTool();
            }
            if (GUILayout.Button("Reset Settings"))
            {
                ResetSettings();
            }
            EditorGUILayout.EndHorizontal();
            
            serializedObject.ApplyModifiedProperties();
        }
        
        private void CreateDefaultUI()
        {
            // Create a new GameObject for the UI
            GameObject canvas = new GameObject("MeasurementToolCanvas");
            Canvas canvasComponent = canvas.AddComponent<Canvas>();
            canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvas.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            
            // Create UI panel
            GameObject uiPanel = new GameObject("MeasurementUI");
            uiPanel.transform.SetParent(canvas.transform);
            
            RectTransform panelRect = uiPanel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0, 1);
            panelRect.anchorMax = new Vector2(0, 1);
            panelRect.pivot = new Vector2(0, 1);
            panelRect.anchoredPosition = new Vector2(10, -10);
            panelRect.sizeDelta = new Vector2(200, 100);
            
            // Create background panel
            GameObject panel = new GameObject("Panel");
            panel.transform.SetParent(uiPanel.transform);
            RectTransform panelBgRect = panel.AddComponent<RectTransform>();
            panelBgRect.anchorMin = Vector2.zero;
            panelBgRect.anchorMax = Vector2.one;
            panelBgRect.sizeDelta = Vector2.zero;
            
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
            MeasurementToolUI uiController = uiPanel.AddComponent<MeasurementToolUI>();
            
            // Set references via reflection
            uiController.GetType().GetField("distanceText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(uiController, distanceText);
            
            uiController.GetType().GetField("angleText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(uiController, angleText);
            
            uiController.GetType().GetField("uiPanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(uiController, panelRect);
            
            // Make sure the prefabs folder exists
            if (!AssetDatabase.IsValidFolder("Assets/Spectator/MeasurementTool/Prefabs"))
            {
                if (!AssetDatabase.IsValidFolder("Assets/Spectator/MeasurementTool"))
                {
                    AssetDatabase.CreateFolder("Assets/Spectator", "MeasurementTool");
                }
                AssetDatabase.CreateFolder("Assets/Spectator/MeasurementTool", "Prefabs");
            }
            
            // Save as prefab
            string prefabPath = "Assets/Spectator/MeasurementTool/Prefabs/MeasurementToolUI.prefab";
            PrefabUtility.SaveAsPrefabAsset(uiPanel, prefabPath);
            
            // Assign to the manager
            uiPrefabProp.objectReferenceValue = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            measurementCanvasProp.objectReferenceValue = canvasComponent;
            
            // Clean up the scene objects
            DestroyImmediate(canvas);
            
            EditorUtility.DisplayDialog("UI Created", 
                "A default UI has been created and assigned to the Measurement Tool Manager.", 
                "OK");
        }
        
        private void ResetSettings()
        {
            toggleKeyProp.enumValueIndex = (int)KeyCode.M;
            measurementColorProp.colorValue = Color.yellow;
            lineWidthProp.floatValue = 2f;
            measurableLayersProp.intValue = -1; // All layers
            
            serializedObject.ApplyModifiedProperties();
        }
        
        private void TestTool()
        {
            MeasurementToolManager manager = (MeasurementToolManager)target;
            
            // Call the toggle method via reflection
            System.Reflection.MethodInfo toggleMethod = typeof(MeasurementToolManager).GetMethod("ToggleMeasurementTool", 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                
            if (toggleMethod != null)
            {
                toggleMethod.Invoke(manager, null);
                
                EditorUtility.DisplayDialog("Tool Activated", 
                    "The Measurement Tool has been activated for testing.\n\n" +
                    "Click two points to measure distance.\n" +
                    "Hold Shift while dragging to measure angles.", 
                    "OK");
            }
        }
        
        private void FindPointMarker()
        {
            string prefabPath = "Assets/Spectator/MeasurementTool/Prefabs/PointMarker.prefab";
            GameObject pointMarker = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            
            if (pointMarker != null)
            {
                pointMarkerPrefabProp.objectReferenceValue = pointMarker;
                serializedObject.ApplyModifiedProperties();
            }
        }
        
        private void FindAngleMarker()
        {
            string prefabPath = "Assets/Spectator/MeasurementTool/Prefabs/AngleMarker.prefab";
            GameObject angleMarker = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            
            if (angleMarker != null)
            {
                angleMarkerPrefabProp.objectReferenceValue = angleMarker;
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif
} 