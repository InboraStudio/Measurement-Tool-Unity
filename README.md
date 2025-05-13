# Measurement Tool

A Unity tool for measuring distances and angles in your scenes. Ideal for game designers, level designers, and anyone who needs precise measurements in their Unity projects.

![image](https://github.com/user-attachments/assets/95c21cc0-6bd9-41c7-9003-f43caf649842)


## Features

- **Distance Measurement**: Click two points to measure the distance between them
- **Angle Measurement**: Hold Shift while dragging to measure angles (displayed in degrees)
- **Visual Feedback**: Clear visual markers and lines show the measured paths
- **UI Display**: Distance and angle measurements are displayed in an easy-to-read UI
- **Easy Toggle**: Press 'M' key to toggle the tool on/off (customizable)
- **Customizable Look**: Change colors, line width, and appearance to match your project
- **Mouse Tracker Integration**: Easily integrate with the PhotoLabs MouseTracker

## Installation

1. Import the `MeasurementTool` folder into your Unity project's Asset folder
2. Add the `MeasurementToolManager` component to any GameObject in your scene
---
![image](https://github.com/user-attachments/assets/9e3b7d81-d8f8-4fe3-a778-765dba5816f3)


## Quick Setup

The fastest way to get started:

### Method 1: Using the Menu (Recommended)

1. In Unity, go to `PhotoLabs > Spectator > Add Measurement Tool`
2. The tool will be added to your scene automatically with all necessary components
3. Press play and use the 'M' key to toggle the tool

### Method 2: Manual Setup

1. Create an empty GameObject in your scene
2. Add the `MeasurementToolManager` component to it
3. Press play and use the 'M' key to toggle the tool

## Integration with Mouse Tracker

If you want to add the Measurement Tool to the existing Mouse Tracker:

1. In Unity, go to `PhotoLabs > Spectator > Add Measurement Tool To Mouse Tracker`
2. The tool will be integrated with your existing MouseTracker
3. Press play and use the 'M' key to toggle the tool

You can also manually add the `MeasurementToolManager` component to your MouseTracker GameObject.

## Usage

- **Toggle Tool**: Press 'M' key (or your custom keybind) to activate/deactivate
- **Measure Distance**: 
  - Click once to place the first point
  - Move to the desired location
  - Click again to place the second point and measure distance
- **Measure Angles**:
  - Hold Shift
  - Click and drag from the first point to the second (angle vertex)
  - Release and move to the third point
  - Click to complete the angle measurement

## Customization

The `MeasurementToolManager` component has several customizable properties:

- **Toggle Key**: Change the key used to activate/deactivate the tool
- **Measurement Color**: Set the color of the measurement lines and indicators
- **Line Width**: Adjust the thickness of the measurement lines
- **Measurable Layers**: Specify which layers can be measured (default: all layers)
- **UI References**: Customize the UI by providing your own prefabs
- **Point Markers**: Use custom prefabs for the measurement points
- **Angle Markers**: Use custom prefabs for the angle visualization

## Advanced Usage

### Custom UI

You can create your own UI prefab and assign it to the `uiPrefab` field of the `MeasurementToolManager`. The prefab should contain:

- A TextMeshProUGUI component for distance display
- A TextMeshProUGUI component for angle display

### Custom Point and Angle Markers

Create custom prefabs for the measurement points and angle visualization and assign them to the respective fields in the `MeasurementToolManager`.

## Troubleshooting

### I can't see the tool in my scene

- Make sure you have added the `MeasurementToolManager` component to an active GameObject
- Press the 'M' key to toggle the tool ON (it's OFF by default)
- Check if the tool is active in the Hierarchy window

### The tool doesn't show up in the Mouse Tracker

- Use the menu option `PhotoLabs > Spectator > Add Measurement Tool To Mouse Tracker`
- Ensure the MouseTracker GameObject is named "MouseTracker" exactly
- Make sure TextMeshPro is installed in your project

## Requirements

- Unity 2019.4 or newer
- TextMeshPro package

## Credits

Developed by Dr Chamyoung for use in Unity projects. Feel free to modify and extend this tool to suit your needs. 
