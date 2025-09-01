using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HanadayoRigidobody : MonoBehaviour
{
    public Rigidbody rb { get { return GetRigidbody(); } }
    public bool isKnockBack { get { return m_isKnockBack; } }
    public bool onGround { get { return m_onGround; } }

    public float frictionValue = 2.0f;
    public float upDacay = 10f;
    public float downDeacay = 6f;
    public float maxFallSpeed = 20f;

    [SerializeField] Vector3 debugVelocity;

    private Rigidbody m_rb;
    protected bool m_isKnockBack;
    protected bool m_onGround;
    // Start is called before the first frame update
    void Start()
    {
        m_isKnockBack = false;
        GetRigidbody();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            KnockBack(new Vector3(10, 20, 0));
        }
    }

    private void LateUpdate()
    {
        Vector3 velocity = m_rb.velocity;
        float ySpeed = velocity.y - ((velocity.y >= 0f) ? upDacay : downDeacay);
        ySpeed = Mathf.Max(ySpeed, -maxFallSpeed);
        m_rb.velocity = new Vector3(velocity.x, ySpeed, velocity.z);
        debugVelocity = m_rb.velocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!m_onGround && collision.gameObject.CompareTag("Ground"))
        {
            m_onGround = true;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            m_isKnockBack = false;
            FrictionUpdate();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (m_onGround && collision.gameObject.CompareTag("Ground"))
        {
            m_onGround = false;
        }
    }

    /// <summary>
    /// ノックバックを発生させる。ノックバック中はisKnockBackがtrueになる。
    /// </summary>
    /// <param name="velocity">方向と強さ</param>
    public void KnockBack(Vector3 velocity)
    {
        Quaternion rotation = Quaternion.LookRotation(new Vector3(-velocity.x, 0f, -velocity.z));
        m_isKnockBack = true;
        StartCoroutine(enumerator());

        IEnumerator enumerator()
        {
            transform.rotation = rotation;
            m_rb.velocity = velocity;
            yield return new WaitWhile(() => m_onGround);
            yield return new WaitForFixedUpdate();
            transform.rotation = rotation;
            m_rb.velocity = velocity;
        }
    }

    /// <summary>
    /// 同じオブジェクトについているRigidbodyを取得する。
    /// </summary>
    /// <returns></returns>
    public Rigidbody GetRigidbody()
    {
        if (!m_rb)
        {
            m_rb = GetComponent<Rigidbody>();
            if (!m_rb)
            {
                Debug.LogError("Rigidbody is not exsist.", this);
                return null;
            }
        }
        return m_rb;
    }

    public static Vector3 GetKnockBackAxis(Vector3 self, Vector3 other)
    {
        Vector3 difference = (other - self);
        Vector3 axis = (new Vector3(difference.x, 0f, difference.z).normalized + Vector3.up).normalized;
        return axis;
    }
    public static Vector3 GetPushAxis(Vector3 self, Vector3 other)
    {
        Vector3 difference = (other - self);
        Vector3 axis = new Vector3(difference.x, 0f, difference.z).normalized;
        return axis;
    }

    private void FrictionUpdate()
    {
        var newVelocity = new Vector3(0f, m_rb.velocity.y, 0f);

        m_rb.velocity = Vector3.Lerp(m_rb.velocity, newVelocity, frictionValue * Time.fixedDeltaTime);
    }
}
