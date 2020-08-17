﻿using System;
using UnityEngine;
using UnityEngine.StreamingImageSequence;

namespace UnityEditor.StreamingImageSequence {

internal static class InspectorUtility {    
    internal static void ShowFrameMarkersGUI(BaseTimelineClipSISDataPlayableAsset timelineClipSISDataPlayableAsset) {        

        TimelineClipSISData timelineClipSISData = timelineClipSISDataPlayableAsset.GetBoundTimelineClipSISData();
        if (null == timelineClipSISData)
            return;
                
        GUILayout.Space(15);
        bool prevMarkerVisibility = timelineClipSISData.AreFrameMarkersVisible();
        bool markerVisibility     = GUILayout.Toggle(prevMarkerVisibility, "Show FrameMarkers");
        if (markerVisibility != prevMarkerVisibility) {
            timelineClipSISData.ShowFrameMarkers(markerVisibility);
        }
            
            
        if (GUILayout.Button("Reset FrameMarkers")) {
            timelineClipSISDataPlayableAsset.ResetPlayableFrames();
        }            
    }

//----------------------------------------------------------------------------------------------------------------------
    
    internal static string ShowFolderSelectorGUI(string label, 
        string dialogTitle, 
        string fieldValue, 
        Func<string, string> onValidFolderSelected)
    {

        string newDirPath = null;
        using(new EditorGUILayout.HorizontalScope()) {
            if (!string.IsNullOrEmpty (label)) {
                EditorGUILayout.PrefixLabel(label);
            } 

            EditorGUILayout.SelectableLabel(fieldValue,
                EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight)
            );

            //Drag drop
            Rect folderRect = GUILayoutUtility.GetLastRect();
        
            Event evt = Event.current;
            switch (evt.type) {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!folderRect.Contains (evt.mousePosition))
                        return fieldValue;
     
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    if (evt.type == EventType.DragPerform) {
                        DragAndDrop.AcceptDrag ();
    
                        if (DragAndDrop.paths.Length <= 0)
                            break;
                        fieldValue = DragAndDrop.paths[0];
//                            onDragAndDrop(DragAndDrop.paths[0]);
                    }

                    break;
                default:
                    break;
            }
                
            
            newDirPath = InspectorUtility.ShowSelectFolderButton(dialogTitle, fieldValue, onValidFolderSelected);

            if (GUILayout.Button("Show", GUILayout.Width(50f))) {
                EditorUtility.RevealInFinder(newDirPath);
            }
            
        }
        
        using (new EditorGUILayout.HorizontalScope()) {
            GUILayout.FlexibleSpace();
            EditorGUI.BeginDisabledGroup(!newDirPath.StartsWith("Assets/"));        
            if(GUILayout.Button("Highlight in Project Window", GUILayout.Width(180f))) {
                AssetEditorUtility.PingAssetByPath(newDirPath);
            }                
            EditorGUI.EndDisabledGroup();
        }
        
        return newDirPath;
    }
    
    
//----------------------------------------------------------------------------------------------------------------------
    
    private static string ShowSelectFolderButton(string title, string folderPath, Func<string, string> onValidFolderSelected) {
        if(GUILayout.Button("Select", GUILayout.Width(50f))) {
            string folderSelected = EditorUtility.OpenFolderPanel(title, folderPath, "");
            if(!string.IsNullOrEmpty(folderSelected)) {
                string newDirPath = null;                    
                if (onValidFolderSelected != null) {
                    newDirPath = onValidFolderSelected (folderSelected);
                } else {
                    newDirPath = folderSelected;
                }

                return newDirPath;
            } else {
                GUIUtility.ExitGUI(); //prevent error when cancel is pressed                
            }
        }

        return folderPath;
    }
    
}

} //end namespace

