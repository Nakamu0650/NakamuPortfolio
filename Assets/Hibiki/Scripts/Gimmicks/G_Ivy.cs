using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_Ivy : MonoBehaviour
{
    public enum IvyType
    {
        Anything,OnlyRoseSword
    }
    [SerializeField] IvyType type;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDamage(int damage)
    {
        switch (type)
        {
            case IvyType.Anything:
                Destroy(gameObject);
                break;
            case IvyType.OnlyRoseSword:
                Destroy(gameObject);
                break;
            default:
                break;
        }
    }
}
