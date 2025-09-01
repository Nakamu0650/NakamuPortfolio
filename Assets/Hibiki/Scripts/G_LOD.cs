using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_LOD : MonoBehaviour
{
    public bool isRun = false;
    [SerializeField] int updateDirection = 3;
    public LayerMask hitLayer;
    public float maxDistance = 500f;
    private Transform cameraTransform;
    private List<MeshRenderer> meshes;

    private Vector3Int cameraPosition;
    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = Camera.main.transform;
        meshes = new List<MeshRenderer>();
        GetAllChildren(transform);
        cameraPosition = Vector3Int.RoundToInt(cameraTransform.position);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isRun) return;

        var nowPosition = Vector3Int.RoundToInt(cameraTransform.position);

        int sum = AbsSumInt(nowPosition - cameraPosition);
        if(sum >= updateDirection)
        {
            cameraPosition = nowPosition;
            SetMeshes(IsVisible());
        }
    }

    private float AbsSum(Vector3 direction)
    {
        return (Mathf.Abs(direction.x) + Mathf.Abs(direction.y) + Mathf.Abs(direction.z));
    }
    private float Sum(Vector3 direction)
    {
        return (direction.x + direction.y + direction.z);
    }
    private int AbsSumInt(Vector3Int direction)
    {
        return (Mathf.Abs(direction.x) + Mathf.Abs(direction.y) + Mathf.Abs(direction.z));
    }
    private int SumInt(Vector3Int direction)
    {
        return (direction.x + direction.y + direction.z);
    }

    private void GetAllChildren(Transform parent)
    {
        foreach(Transform child in parent)
        {
            var mesh = child.GetComponent<MeshRenderer>();
            if (mesh != null)
            {
                meshes.Add(mesh);
            }
            GetAllChildren(child);
        }
    }

    private bool IsVisible()
    {
        Vector3 direction = (cameraTransform.position - transform.position);

        if(AbsSum(direction) >= maxDistance)
        {
            return false;
        }

        var hits = Physics.RaycastAll(transform.position, direction.normalized, Vector3.Magnitude(direction) - 1f, hitLayer);

        foreach(var hit in hits)
        {
            if (hit.collider.gameObject.CompareTag("Ground"))
            {
                return false;
            }
        }
        return true;
    }

    private void SetMeshes(bool active)
    {
        foreach(var mesh in meshes)
        {
            mesh.enabled = active;
        }
    }
}
