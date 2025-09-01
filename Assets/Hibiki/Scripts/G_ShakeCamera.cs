using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class G_ShakeCamera : MonoBehaviour
{
    public static G_ShakeCamera instance;
    [SerializeField] Vector2 damageShakeForce;
    [SerializeField] Vector2 damageRange;

    private CinemachineImpulseSource impulseSource;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void ShakeCamera(float velocity)
    {
        impulseSource.GenerateImpulseWithForce(velocity);
    }
    public void ShakeCamera(Vector3 velocity)
    {
        impulseSource.GenerateImpulseWithVelocity(velocity);
    }

    public void DamageShake(int damage)
    {
        float lerp = Mathf.InverseLerp((float)damage, damageRange.x, damageRange.y);
        float force = Mathf.Lerp(lerp, damageShakeForce.x, damageShakeForce.y);
        impulseSource.GenerateImpulseWithVelocity(Vector3.up * force);
    }
}
