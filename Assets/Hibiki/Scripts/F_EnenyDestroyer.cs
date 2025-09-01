using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F_EnenyDestroyer : MonoBehaviour
{
    private F_HP hp;
    [SerializeField] Renderer modelRenderer;
    [SerializeField] float destroyLifeTime = 10f;
    // Start is called before the first frame update
    void Start()
    {
        hp = GetComponent<F_HP>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hp.isKilled)
        {
            if (destroyLifeTime > 0f)
            {
                destroyLifeTime -= Time.deltaTime;
            }
            else if (!modelRenderer.isVisible)
            {
                Destroy(gameObject);
            }
        }
    }
}
