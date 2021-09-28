#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

using UnityEngine;

namespace NavigationPath
{
    #if UNITY_EDITOR
    [CustomEditor(typeof(BezierPath))]
    public class BezierPathEditor : Editor
    {
        private BezierPath path;
        private float handleScale = 0.2f;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDefaultInspector();
            EditorGUILayout.Space();

            if (GUILayout.Button("Add Cubic Bezier Segment"))
            {
                Undo.RecordObject(path, "Create New");
                
                var endPoint = CalculateEndPoint();

                if (path.Length == 0)
                    path.CreateFirstCubicBezierSegment(path.transform.position, endPoint);
                else
                    path.AddCubicBezierSegment(endPoint);
                
                RecordObjectMOdification();
            }

            if (GUILayout.Button("Add Quadratic Bezier Segment"))
            {
                Undo.RecordObject(path, "Create New");
                
                var endPoint = CalculateEndPoint();

                if (path.Length == 0)
                    path.CreateFirstQuadraticBezierSegment(path.transform.position, endPoint);
                else
                    path.AddQuadraticBezierSegment(endPoint);
                
                RecordObjectMOdification();
            }

            if (GUILayout.Button("Add Linear Bezier Segment"))
            {
                Undo.RecordObject(path, "Create New");
                
                var endPoint = CalculateEndPoint();

                if (path.Length == 0)
                    path.CreateFirstLinearBezierSegment(path.transform.position, endPoint);
                else
                    path.AddLinearBezierSegment(endPoint);
                
                RecordObjectMOdification();
            }
            
            if (GUILayout.Button("Remove Last Segment"))
            {
                Undo.RecordObject(path, "Remove Last Segment");
                
                path.RemoveLastSegment();
                
                RecordObjectMOdification();
            }

            if (path.IsClosed)
            {
                if (GUILayout.Button("Break Path"))
                {
                    Undo.RecordObject(path, "Break Path");
                    path.Break();
                    RecordObjectMOdification();
                }
            }
            else
            {
                if (GUILayout.Button("Close Path"))
                {
                    Undo.RecordObject(path, "Close Path");
                    path.Close();
                    RecordObjectMOdification();
                }
            }

            if (GUILayout.Button("Clear"))
                path.Clear();

            serializedObject.ApplyModifiedProperties();
        }

        private void OnSceneGUI()
        {
            DetectHandlesMovement();
            
            path.ForeachSegment(DrawSegment);

            if (Event.current.shift)
                path.ForeachSegment(DrawConnector);
        }

        private Vector3 CalculateEndPoint()
        {
            if (path.Length == 0)
                return path.transform.position + path.transform.forward * HandleUtility.GetHandleSize(path.transform.position) * 10f;
            
            var currentDirection = path.TailSegment.AnchorPointB.Position - path.TailSegment.AnchorPointA.Position;

            return path.TailSegment.AnchorPointB.Position + currentDirection;
        }

        private void DrawConnector(BezierSegment segment)
        {
            if (segment is CubicBezierSegment a && segment.NextSegment is CubicBezierSegment b)
            {
                Handles.color = (a.IsBrokenB && b.IsBrokenA) ? Color.red : Color.green;
                
                var position = a.AnchorPointB.Position;
                var handleSize = HandleUtility.GetHandleSize(position) * handleScale;
                
                if (Handles.Button(position, Quaternion.identity, handleSize, handleSize, Handles.RectangleHandleCap))
                    a.IsBrokenB = !a.IsBrokenB;
            }
        }

        private void DrawSegment(BezierSegment segment)
        {
            switch (segment)
            {
                case LinearBezierSegment s:
                    DrawStraightLine(s);
                    break;
                case QuadraticBezierSegment s:
                    DrawQuadraticBezierCurve(s);
                    break;
                case CubicBezierSegment s:
                    DrawCubicBezierCurve(s);
                    break;
            }
        }

        private void DrawStraightLine(LinearBezierSegment segment)
        {
            Handles.color = Color.green;
            Handles.DrawLine(segment.AnchorPointA.Position, segment.AnchorPointB.Position);
        }

        private void DrawQuadraticBezierCurve(QuadraticBezierSegment segment)
        {
            Handles.color = Color.red;
            Handles.DrawDottedLine(segment.AnchorPointA.Position, segment.LerpPoint.Position, 1);
            Handles.DrawDottedLine(segment.AnchorPointB.Position, segment.LerpPoint.Position, 1);
            Handles.DrawBezier(segment.AnchorPointA.Position,
                               segment.AnchorPointB.Position,
                               segment.LerpPoint.Position,
                               segment.LerpPoint.Position,
                               Color.green, null, 2);
        }

        private void DrawCubicBezierCurve(CubicBezierSegment segment)
        {
            Handles.color = Color.red;
            Handles.DrawDottedLine(segment.AnchorPointA.Position, segment.LerpPointA.Position, 1);
            Handles.DrawDottedLine(segment.AnchorPointB.Position, segment.LerpPointB.Position, 1);
            Handles.DrawBezier(segment.AnchorPointA.Position,
                               segment.AnchorPointB.Position,
                               segment.LerpPointA.Position,
                               segment.LerpPointB.Position,
                               Color.green, null, 2);
        }

        private void DetectHandlesMovement()
        {
            path.ForeachControlPoint(HandleControlPoint);
        }

        private void HandleControlPoint(ControlPoint controlPoint)
        {
            Handles.color = Color.red;
            
            var handleSize  = HandleUtility.GetHandleSize(controlPoint.Position) * handleScale;
            var newPosition = Handles.FreeMoveHandle(controlPoint.Position, Quaternion.identity, handleSize, Vector2.zero, Handles.SphereHandleCap);
            
            if (newPosition != controlPoint.Position)
            {
                Undo.RecordObject(path, "Move Control Point");
                controlPoint.Position = newPosition;
                RecordObjectMOdification();
            }
        }

        private void RecordObjectMOdification()
        {
            // if (EditorApplication.isPlaying)
            //     return;
            //
            // EditorUtility.SetDirty(path);
            // EditorSceneManager.MarkSceneDirty(path.gameObject.scene);
            // PrefabUtility.RecordPrefabInstancePropertyModifications(path);
        }

        private void OnEnable()
        {
            path = (BezierPath)target;
        }   
    }
    #endif
}
