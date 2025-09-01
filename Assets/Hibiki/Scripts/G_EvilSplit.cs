using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class G_EvilSplit : MonoBehaviour
{
    [SerializeField] F_Param paramater;
    [SerializeField] F_AttackValue evilSplit;
    [SerializeField] float groundHitRadius = 0.1f;
    public float speed = 10f;
    [SerializeField] string enemyTagName = "Player", groundTagName = "Ground";
    [SerializeField] LayerMask enemyLayer, groundLayer;
    [SerializeField] Vector3 colliderSize;
    public bool isGo = false;
    public UnityEvent onHitGround = new UnityEvent();

    private List<GameObject> hitEnemies;
    private Vector3 beforePos;

    private void Start()
    {
        hitEnemies = new List<GameObject>();
        beforePos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isGo) return;
        transform.position += transform.forward * Time.fixedDeltaTime * speed;

        var enemies = Physics.OverlapBox(transform.position, Vector3.Scale(transform.localScale, colliderSize), transform.rotation, enemyLayer).Where(hit => (hit.gameObject.CompareTag(enemyTagName))).ToList();
        var grounds = Physics.OverlapCapsule(beforePos, transform.position, groundHitRadius, groundLayer).Where(hit => (hit.gameObject.CompareTag(groundTagName))).ToList();
        foreach(var v in Physics.OverlapCapsule(beforePos, transform.position, groundHitRadius, groundLayer))
        {
            Debug.Log(v.gameObject.name, v.gameObject);
        }
        beforePos = transform.position;

        enemies.ForEach(hit =>
        {
            if (!hitEnemies.Contains(hit.gameObject))
            {
                hitEnemies.Add(hit.gameObject);
                hit.gameObject.GetComponent<F_HP>().OnDamage(paramater.GetATK(), evilSplit.attackValues[0], Vector3.zero);
            }
        });

        if(grounds.Count > 0)
        {
            onHitGround.Invoke(transform);
            Destroy(gameObject);
        }
    }

    public class UnityEvent : UnityEngine.Events.UnityEvent<Transform>
    {

    }
}
