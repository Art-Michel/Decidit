using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine.AI;

[EditorTool("NavmeshLinkPlacement", typeof(NavMeshLink))]
public class NavmeshLinkPlacement : EditorTool
{
    public override GUIContent toolbarIcon => EditorGUIUtility.IconContent("Animation.Record");

    Vector3 positionClick;


    public override void OnToolGUI(EditorWindow window)
    {
        if (!(window is SceneView))
            return;

        SceneView _sceneview = window as SceneView;

        Camera cam = _sceneview.camera;

        Event _event = Event.current;

        if (_event.type != EventType.MouseDown)
            return;

        Vector2 mousePosition = Event.current.mousePosition;
        mousePosition.y = cam.pixelHeight- mousePosition.y;

        RaycastHit hit;
        Ray ray;
        ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

        if (_event.button ==1 && _event.shift)
        {
            if (Physics.Raycast(ray, out hit))
            {
                positionClick = CheckNavMeshPoint(hit.point);

                NavMeshLink link = target as NavMeshLink;

                link.startPoint = positionClick;
            }
        }
        else if(_event.button == 1 && _event.control)
        {
            if (Physics.Raycast(ray, out hit))
            {
                positionClick = CheckNavMeshPoint(hit.point);

                NavMeshLink link = target as NavMeshLink;

                link.endPoint = positionClick;
            }
        }

      /* // Handles.BeginGUI();
        Handles.color = Color.green;
        Handles.DrawWireDisc(positionClick, Vector3.up, 5f);
        //Handles.EndGUI();*/
    }

    Vector3 CheckNavMeshPoint(Vector3 _destination)
    {
        NavMeshHit closestHit;
        if (NavMesh.SamplePosition(_destination, out closestHit, 1, 1))
        {
            _destination = closestHit.position;
            Debug.Log(_destination);
           Debug.DrawLine(_destination, Vector3.up*5f, Color.red, 10f);
        }
        return _destination;
    }
}