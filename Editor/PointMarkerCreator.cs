using UnityEngine;
using UnityEditor;

namespace PhotoLabs.Spectator
{
#if UNITY_EDITOR
    public class PointMarkerCreator : MonoBehaviour
    {
        [MenuItem("PhotoLabs/Spectator/Create Measurement Markers/Create Point Marker")]
        public static void CreatePointMarker()
        {
            // Create the point marker
            GameObject pointMarker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            pointMarker.name = "PointMarker";
            pointMarker.transform.localScale = Vector3.one * 0.1f;
            
            // Add a material with a bright color
            Renderer renderer = pointMarker.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material material = new Material(Shader.Find("Standard"));
                material.color = Color.yellow;
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", Color.yellow * 0.5f);
                renderer.material = material;
                
                // Save the material asset
                if (!AssetDatabase.IsValidFolder("Assets/Spectator/MeasurementTool/Materials"))
                {
                    if (!AssetDatabase.IsValidFolder("Assets/Spectator/MeasurementTool"))
                    {
                        AssetDatabase.CreateFolder("Assets/Spectator", "MeasurementTool");
                    }
                    AssetDatabase.CreateFolder("Assets/Spectator/MeasurementTool", "Materials");
                }
                
                AssetDatabase.CreateAsset(material, "Assets/Spectator/MeasurementTool/Materials/PointMarkerMaterial.mat");
            }
            
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
            string prefabPath = "Assets/Spectator/MeasurementTool/Prefabs/PointMarker.prefab";
            PrefabUtility.SaveAsPrefabAsset(pointMarker, prefabPath);
            
            // Clean up the scene object
            DestroyImmediate(pointMarker);
            
            // Show the prefab in the project window
            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath));
            
            EditorUtility.DisplayDialog("Point Marker Created", 
                "A Point Marker prefab has been created at:\n" +
                prefabPath + "\n\n" +
                "You can assign this prefab to the 'Point Marker Prefab' field of any MeasurementToolManager component.", 
                "OK");
        }
        
        [MenuItem("PhotoLabs/Spectator/Create Measurement Markers/Create Angle Marker")]
        public static void CreateAngleMarker()
        {
            // Create the angle marker parent
            GameObject angleMarker = new GameObject("AngleMarker");
            
            // Create a curved line to represent the angle
            GameObject arcObject = new GameObject("AngleArc");
            arcObject.transform.SetParent(angleMarker.transform);
            
            // Add a line renderer to create the arc
            LineRenderer lineRenderer = arcObject.AddComponent<LineRenderer>();
            lineRenderer.startWidth = 0.02f;
            lineRenderer.endWidth = 0.02f;
            lineRenderer.positionCount = 21; // Number of points in the arc
            
            // Create a material for the arc
            Material arcMaterial = new Material(Shader.Find("Standard"));
            arcMaterial.color = new Color(1f, 0.5f, 0f, 1f); // Orange color
            arcMaterial.EnableKeyword("_EMISSION");
            arcMaterial.SetColor("_EmissionColor", new Color(1f, 0.5f, 0f, 1f) * 0.5f);
            lineRenderer.material = arcMaterial;
            
            // Generate an arc of 90 degrees
            float radius = 0.2f;
            for (int i = 0; i < 21; i++)
            {
                float angle = Mathf.Deg2Rad * (i * 90f / 20f); // 90 degree arc
                float x = Mathf.Cos(angle) * radius;
                float z = Mathf.Sin(angle) * radius;
                lineRenderer.SetPosition(i, new Vector3(x, 0, z));
            }
            
            // Add small spheres at the endpoints
            GameObject startSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            startSphere.name = "StartPoint";
            startSphere.transform.SetParent(angleMarker.transform);
            startSphere.transform.localScale = Vector3.one * 0.05f;
            startSphere.transform.localPosition = new Vector3(radius, 0, 0);
            
            GameObject endSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            endSphere.name = "EndPoint";
            endSphere.transform.SetParent(angleMarker.transform);
            endSphere.transform.localScale = Vector3.one * 0.05f;
            endSphere.transform.localPosition = new Vector3(0, 0, radius);
            
            // Apply the same material to the spheres
            startSphere.GetComponent<Renderer>().material = arcMaterial;
            endSphere.GetComponent<Renderer>().material = arcMaterial;
            
            // Save the material asset
            if (!AssetDatabase.IsValidFolder("Assets/Spectator/MeasurementTool/Materials"))
            {
                if (!AssetDatabase.IsValidFolder("Assets/Spectator/MeasurementTool"))
                {
                    AssetDatabase.CreateFolder("Assets/Spectator", "MeasurementTool");
                }
                AssetDatabase.CreateFolder("Assets/Spectator/MeasurementTool", "Materials");
            }
            
            AssetDatabase.CreateAsset(arcMaterial, "Assets/Spectator/MeasurementTool/Materials/AngleMarkerMaterial.mat");
            
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
            string prefabPath = "Assets/Spectator/MeasurementTool/Prefabs/AngleMarker.prefab";
            PrefabUtility.SaveAsPrefabAsset(angleMarker, prefabPath);
            
            // Clean up the scene object
            DestroyImmediate(angleMarker);
            
            // Show the prefab in the project window
            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath));
            
            EditorUtility.DisplayDialog("Angle Marker Created", 
                "An Angle Marker prefab has been created at:\n" +
                prefabPath + "\n\n" +
                "You can assign this prefab to the 'Angle Marker Prefab' field of any MeasurementToolManager component.", 
                "OK");
        }
    }
#endif
} 