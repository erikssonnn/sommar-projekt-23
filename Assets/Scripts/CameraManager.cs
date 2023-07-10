using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private float panSpeed = 0.0f;
    [SerializeField] private float scrollPanSpeed = 0.0f;
    [SerializeField] private float scrollSpeed = 0.0f;

    [SerializeField] private float minZoom, maxZoom = 0.0f;
    [SerializeField] private float panBorder = 0.0f;

    private MapManager mapManager = null;
    private Camera cam = null;
    private const float downScale = 0.1f;
    private Transform t = null;
    private Vector2Int mapLimit = Vector2Int.zero;

    private void Start()
    {
        t = transform;
        mapManager = MapManager.Instance;
        if (mapManager == null)
        {
            throw new System.Exception("mapManager instance is null!");
        }

        cam = Camera.main;
        if (cam == null)
        {
            throw new System.Exception("Cant find main camera!");
        }

        mapLimit = new Vector2Int(mapManager.MapSize.x, mapManager.MapSize.y);
    }

    private void Update()
    {
        if (Input.GetMouseButton(2))
        {
            ScrollMovement();
            return;
        }

        Zoom();
        Movement();
    }

    private void Zoom()
    {
        float size = cam.orthographicSize;
        float scroll = Input.GetAxisRaw("Mouse ScrollWheel");
        size -= scroll * scrollSpeed * Time.fixedDeltaTime;
        size = Mathf.Clamp(size, minZoom, maxZoom);
        cam.orthographicSize = size;
    }

    private void ScrollMovement()
    {
        float x = Input.GetAxisRaw("Mouse X");
        float y = Input.GetAxisRaw("Mouse Y");
        Vector3 pos = transform.position -= new Vector3(x, 0, y) *
                                            (scrollPanSpeed * Time.fixedDeltaTime * cam.orthographicSize * downScale);
        pos.x = Mathf.Clamp(pos.x, -mapLimit.x, mapLimit.x);
        pos.z = Mathf.Clamp(pos.z, -mapLimit.y, mapLimit.y);
        t.position = pos;
    }

    private void Movement()
    {
        Vector3 pos = transform.position;

        if (Input.GetKey(KeyCode.W) || Input.mousePosition.y >= Screen.height - panBorder)
        {
            pos.z += (panSpeed * Time.fixedDeltaTime * cam.orthographicSize * downScale);
        }
        if (Input.GetKey(KeyCode.S) || Input.mousePosition.y <= panBorder)
        {
            pos.z -= (panSpeed * Time.fixedDeltaTime * cam.orthographicSize * downScale);
        }
        if (Input.GetKey(KeyCode.D) || Input.mousePosition.x >= Screen.width - panBorder)
        {
            pos.x += (panSpeed * Time.fixedDeltaTime * cam.orthographicSize * downScale);
        }
        if (Input.GetKey(KeyCode.A) || Input.mousePosition.x <= panBorder)
        {
            pos.x -= (panSpeed * Time.fixedDeltaTime * cam.orthographicSize * downScale);
        }
        
        pos.x = Mathf.Clamp(pos.x, -mapLimit.x, mapLimit.x);
        pos.z = Mathf.Clamp(pos.z, -mapLimit.y, mapLimit.y);
        t.position = pos;
    }
}