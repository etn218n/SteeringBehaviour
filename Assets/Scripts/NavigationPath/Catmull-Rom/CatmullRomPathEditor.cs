#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

using System.Collections.Generic;
using UnityEngine;

namespace NavigationPath
{
    #if UNITY_EDITOR
    [CustomEditor(typeof(CatmullRomPath))]
    public class CatmullRomPathEditor : Editor
    {
        private float handleScale = 0.2f;
        private CatmullRomPath path;
        private List<Vector3> points = new List<Vector3>();

        private void OnEnable()
        {
            path = (CatmullRomPath)target;
        }  
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
        
            DrawDefaultInspector();
            EditorGUILayout.Space();
        
            if (GUILayout.Button("Add Segment"))
            {
                Undo.RecordObject(path, "Create New");
                
                var endPoint = CalculateEndPoint();

                if (path.IsEmpty)
                    path.CreateFirstSegment(path.transform.position, endPoint);
                else
                    path.AddSegment(endPoint);
                
                points = path.GeneratePoints();

                RecordObjectMOdification();
            }
            
            if (GUILayout.Button("Remove Last Segment"))
            {
                Undo.RecordObject(path, "Remove Last Segment");
                
                path.RemoveLastSegment();
                
                points = path.GeneratePoints();
                
                RecordObjectMOdification();
            }

            if (GUILayout.Button("Clear"))
            {
                path.Clear();
                points.Clear();
            }
        
            serializedObject.ApplyModifiedProperties();
        }
        
        private void OnSceneGUI()
        {
            if (!path.IsEmpty && points.Count == 0)
                points = path.GeneratePoints();

            DetectHandlesMovement();
            DrawCatmullRomCurve();
        }
        
        private Vector3 CalculateEndPoint()
        {
            if (path.IsEmpty)
                return path.transform.position + Vector3.one * HandleUtility.GetHandleSize(path.transform.position) * 0.1f;
            
            return points[points.Count - 1] + Vector3.one * HandleUtility.GetHandleSize(path.transform.position) * 0.1f;
        }

        private void DrawCatmullRomCurve()
        {
            Handles.color = Color.green;

            for (int i = 0; i <= points.Count - 2; i++)
                Handles.DrawLine(points[i], points[i + 1]);
        }
        
        private void DetectHandlesMovement()
        {
            path.ForEachControlPoint(HandleControlPoint);
        }
        
        private void HandleControlPoint(Vector3 controlPoint, int index)
        {
            Handles.color = Color.red;
            
            var handleSize  = HandleUtility.GetHandleSize(controlPoint) * handleScale;
            var newPosition = Handles.FreeMoveHandle(controlPoint, Quaternion.identity, handleSize, Vector2.zero, Handles.SphereHandleCap);
            
            if (newPosition != controlPoint)
            {
                Undo.RecordObject(path, "Move Control Point");
                path.MoveControlPoint(index, newPosition);
                points = path.GeneratePoints();
                RecordObjectMOdification();
            }
        }

        private void RecordObjectMOdification()
        {
            if (EditorApplication.isPlaying)
                return;
            
            EditorUtility.SetDirty(path);
            EditorSceneManager.MarkSceneDirty(path.gameObject.scene);
            PrefabUtility.RecordPrefabInstancePropertyModifications(path);
        }
    }
    #endif
}
