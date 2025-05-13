using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace PhotoLabs.Spectator
{
    public class MeasurementTool : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject measurementUI;
        [SerializeField] private TextMeshProUGUI distanceText;
        [SerializeField] private TextMeshProUGUI angleText;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private GameObject pointPrefab;
        [SerializeField] private GameObject anglePrefab;
        
        [Header("Settings")]
        [SerializeField] private Color lineColor = Color.yellow;
        [SerializeField] private float lineWidth = 2f;
        [SerializeField] private LayerMask measurableLayers = ~0; // All layers by default
        [SerializeField] private bool showAngleArc = true;
        [SerializeField] private float maxMeasurementDistance = 1000f;
        
        // Private variables
        private Camera mainCamera;
        private Vector3 startPoint;
        private Vector3 endPoint;
        private GameObject startMarker;
        private GameObject endMarker;
        private GameObject angleMarker;
        private bool isFirstPointSet = false;
        private bool isDragging = false;
        private bool isAngleMeasurement = false;
        private Vector3 dragStartPoint;
        private Vector3 middlePoint;
        private List<GameObject> pointMarkers = new List<GameObject>();
        
        private void Awake()
        {
            mainCamera = Camera.main;
            
            // Initialize line renderer if not assigned
            if (lineRenderer == null)
            {
                lineRenderer = gameObject.AddComponent<LineRenderer>();
                lineRenderer.startWidth = lineWidth;
                lineRenderer.endWidth = lineWidth;
                lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
                lineRenderer.startColor = lineColor;
                lineRenderer.endColor = lineColor;
                lineRenderer.positionCount = 0;
            }
            
            // Initialize UI if needed
            if (measurementUI == null)
            {
                Debug.LogWarning("MeasurementTool: UI reference not set. Distance and angle won't be displayed.");
            }
            
            // Hide UI when starting
            if (measurementUI != null)
            {
                measurementUI.SetActive(false);
            }
        }
        
        private void Update()
        {
            // Cancel measurement if Escape key is pressed
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ClearMeasurement();
                return;
            }
            
            // Check if we're over UI
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            
            // Check if Shift is held down for angle measurement
            isAngleMeasurement = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            
            // Handle mouse input
            HandleMouseInput();
            
            // Update UI
            UpdateUI();
        }
        
        private void HandleMouseInput()
        {
            // For distance measurement (no shift)
            if (!isAngleMeasurement)
            {
                // Left mouse button pressed - place the first point
                if (Input.GetMouseButtonDown(0) && !isFirstPointSet)
                {
                    if (TryGetMouseWorldPoint(out Vector3 hitPoint))
                    {
                        startPoint = hitPoint;
                        isFirstPointSet = true;
                        
                        // Create marker at start point
                        startMarker = CreatePointMarker(startPoint);
                        pointMarkers.Add(startMarker);
                        
                        // Initialize line renderer for drawing
                        lineRenderer.positionCount = 2;
                        lineRenderer.SetPosition(0, startPoint);
                        lineRenderer.SetPosition(1, startPoint);
                        
                        // Show UI
                        if (measurementUI != null)
                        {
                            measurementUI.SetActive(true);
                        }
                    }
                }
                // Left mouse button released after first point - place the second point
                else if (Input.GetMouseButtonUp(0) && isFirstPointSet && !isDragging)
                {
                    if (TryGetMouseWorldPoint(out Vector3 hitPoint))
                    {
                        endPoint = hitPoint;
                        
                        // Create marker at end point
                        endMarker = CreatePointMarker(endPoint);
                        pointMarkers.Add(endMarker);
                        
                        // Update line renderer
                        lineRenderer.SetPosition(1, endPoint);
                        
                        // Reset first point flag to allow new measurement
                        isFirstPointSet = false;
                    }
                }
                // Mouse move with first point set - update the line
                else if (isFirstPointSet && !Input.GetMouseButton(0))
                {
                    if (TryGetMouseWorldPoint(out Vector3 hitPoint))
                    {
                        endPoint = hitPoint;
                        lineRenderer.SetPosition(1, endPoint);
                    }
                }
            }
            // For angle measurement (with shift)
            else
            {
                // Left mouse button pressed - start drag
                if (Input.GetMouseButtonDown(0) && !isDragging)
                {
                    if (TryGetMouseWorldPoint(out Vector3 hitPoint))
                    {
                        dragStartPoint = hitPoint;
                        isDragging = true;
                        
                        // Create marker at drag start point
                        startMarker = CreatePointMarker(dragStartPoint);
                        pointMarkers.Add(startMarker);
                        
                        // Initialize line renderer for drawing
                        lineRenderer.positionCount = 3;
                        lineRenderer.SetPosition(0, dragStartPoint);
                        lineRenderer.SetPosition(1, dragStartPoint);
                        lineRenderer.SetPosition(2, dragStartPoint);
                        
                        // Show UI
                        if (measurementUI != null)
                        {
                            measurementUI.SetActive(true);
                        }
                    }
                }
                // Left mouse button held - update middle point
                else if (isDragging && Input.GetMouseButton(0))
                {
                    if (TryGetMouseWorldPoint(out Vector3 hitPoint))
                    {
                        middlePoint = hitPoint;
                        
                        // If we don't yet have a middle marker, create one
                        if (endMarker == null)
                        {
                            endMarker = CreatePointMarker(middlePoint);
                            pointMarkers.Add(endMarker);
                        }
                        else
                        {
                            endMarker.transform.position = middlePoint;
                        }
                        
                        // Update line renderer
                        lineRenderer.SetPosition(1, middlePoint);
                        lineRenderer.SetPosition(2, middlePoint);
                    }
                }
                // Left mouse button released - complete angle
                else if (isDragging && Input.GetMouseButtonUp(0))
                {
                    if (TryGetMouseWorldPoint(out Vector3 hitPoint))
                    {
                        endPoint = hitPoint;
                        
                        // Create marker at end point
                        var finalMarker = CreatePointMarker(endPoint);
                        pointMarkers.Add(finalMarker);
                        
                        // Update line renderer
                        lineRenderer.SetPosition(2, endPoint);
                        
                        // Create angle arc visualization
                        if (showAngleArc)
                        {
                            CreateAngleVisualization();
                        }
                        
                        // Reset drag flag to allow new measurement
                        isDragging = false;
                    }
                }
                // Mouse move with dragging - update the end point
                else if (isDragging && !Input.GetMouseButton(0))
                {
                    if (TryGetMouseWorldPoint(out Vector3 hitPoint))
                    {
                        endPoint = hitPoint;
                        lineRenderer.SetPosition(2, endPoint);
                    }
                }
            }
        }
        
        private bool TryGetMouseWorldPoint(out Vector3 hitPoint)
        {
            hitPoint = Vector3.zero;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, maxMeasurementDistance, measurableLayers))
            {
                hitPoint = hit.point;
                return true;
            }
            
            // If no hit with physics, use a plane at a fixed distance from the camera
            Plane plane = new Plane(mainCamera.transform.forward, 10f);
            float distance;
            if (plane.Raycast(ray, out distance))
            {
                hitPoint = ray.GetPoint(distance);
                return true;
            }
            
            return false;
        }
        
        private GameObject CreatePointMarker(Vector3 position)
        {
            GameObject marker;
            
            if (pointPrefab != null)
            {
                marker = Instantiate(pointPrefab, position, Quaternion.identity, transform);
            }
            else
            {
                // Create a simple sphere if no prefab is provided
                marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                marker.transform.position = position;
                marker.transform.localScale = Vector3.one * 0.1f;
                marker.transform.SetParent(transform);
                
                // Set color
                Renderer renderer = marker.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = lineColor;
                }
            }
            
            return marker;
        }
        
        private void CreateAngleVisualization()
        {
            if (angleMarker != null)
            {
                Destroy(angleMarker);
            }
            
            if (anglePrefab != null)
            {
                angleMarker = Instantiate(anglePrefab, middlePoint, Quaternion.identity, transform);
                
                // Adjust the angle marker's rotation to match the measured angle
                Vector3 dir1 = (dragStartPoint - middlePoint).normalized;
                Vector3 dir2 = (endPoint - middlePoint).normalized;
                
                // Get angle between vectors
                float angle = Vector3.SignedAngle(dir1, dir2, Vector3.up);
                
                // Apply rotation
                angleMarker.transform.forward = dir1;
                angleMarker.transform.Rotate(Vector3.up, angle);
            }
            else
            {
                // If no angle prefab, just add the angle marker to the markers list 
                // so it gets deleted with everything else
                angleMarker = new GameObject("AngleMarker");
                angleMarker.transform.SetParent(transform);
                angleMarker.transform.position = middlePoint;
                pointMarkers.Add(angleMarker);
            }
        }
        
        private void UpdateUI()
        {
            if (measurementUI == null)
                return;
                
            if (!isAngleMeasurement)
            {
                // Update distance text
                if (distanceText != null && isFirstPointSet)
                {
                    float distance = Vector3.Distance(startPoint, endPoint);
                    distanceText.text = $"Distance: {distance:F2} units";
                    distanceText.gameObject.SetActive(true);
                }
                
                // Hide angle text
                if (angleText != null)
                {
                    angleText.gameObject.SetActive(false);
                }
            }
            else
            {
                // Calculate and update angle text
                if (angleText != null && isDragging)
                {
                    // Calculate angle
                    Vector3 dir1 = (dragStartPoint - middlePoint).normalized;
                    Vector3 dir2 = (endPoint - middlePoint).normalized;
                    float angle = Vector3.Angle(dir1, dir2);
                    
                    angleText.text = $"Angle: {angle:F1}°";
                    angleText.gameObject.SetActive(true);
                }
                
                // Update distance text for both segments
                if (distanceText != null && isDragging)
                {
                    float distance1 = Vector3.Distance(dragStartPoint, middlePoint);
                    float distance2 = Vector3.Distance(middlePoint, endPoint);
                    distanceText.text = $"A to B: {distance1:F2}\nB to C: {distance2:F2}";
                    distanceText.gameObject.SetActive(true);
                }
            }
        }
        
        public void ClearMeasurement()
        {
            // Clear line renderer
            if (lineRenderer != null)
            {
                lineRenderer.positionCount = 0;
            }
            
            // Remove all point markers
            foreach (var marker in pointMarkers)
            {
                if (marker != null)
                {
                    Destroy(marker);
                }
            }
            pointMarkers.Clear();
            
            // Remove angle marker
            if (angleMarker != null)
            {
                Destroy(angleMarker);
                angleMarker = null;
            }
            
            // Reset state
            isFirstPointSet = false;
            isDragging = false;
            
            // Hide UI
            if (measurementUI != null)
            {
                measurementUI.SetActive(false);
            }
            
            startMarker = null;
            endMarker = null;
        }
    }
} 