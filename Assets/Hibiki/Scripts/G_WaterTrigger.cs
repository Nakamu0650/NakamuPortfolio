using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class G_WaterTrigger : MonoBehaviour
{
    private Vector3Int point;
    [SerializeField] LayerMask waterLayer;
    [SerializeField] string waterTag = "Water";
    [SerializeField] UnityEvent onWater;
    // Start is called before the first frame update
    void Start()
    {
        point = Vector3Int.RoundToInt(transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3Int position = Vector3Int.RoundToInt(transform.position);
        if (point != position)
        {
            point = position;
            if(Physics.OverlapSphere(position, 1f, waterLayer).Where(col => col.gameObject.CompareTag(waterTag)).ToArray().Length > 0)
            {
                onWater.Invoke();
            }
        }
    }
}
