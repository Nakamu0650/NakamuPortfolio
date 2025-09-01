using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_DebugWorpPoints : MonoBehaviour
{
    [SerializeField] Transform player;
    private List<Transform> worpPoints = new List<Transform>();
    private Dictionary<KeyCode, int> keys = new Dictionary<KeyCode, int> { { KeyCode.Alpha0, 0 },{ KeyCode.Alpha1, 1 }, { KeyCode.Alpha2, 2 }, { KeyCode.Alpha3, 3 }, { KeyCode.Alpha4, 4 }, { KeyCode.Alpha5, 5 }, { KeyCode.Alpha6, 6 }, { KeyCode.Alpha7, 7 }, { KeyCode.Alpha8, 8 }, { KeyCode.Alpha9, 9 } };
    // Start is called before the first frame update
    void Start()
    {
        worpPoints = new List<Transform>();
        foreach(Transform child in transform)
        {
            worpPoints.Add(child);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach(KeyCode key in keys.Keys)
        {
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(key))
            {
                try
                {
                    player.position = worpPoints[keys[key]].position + Vector3.up * 2f;
                }
                catch
                {
                    print("Not match worp point.");
                }
            }
        }
    }
}
