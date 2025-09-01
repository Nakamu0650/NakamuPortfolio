using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_mashroom : MonoBehaviour
{
    public float floatingHeight = 500f; // ��яオ�鍂��
    public float floatingSpeed = 3f; // ��яオ�鑬�x
    public float fallingHeight = 1000f; // �������鍂��
    public float fallingSpeed = 3f; // �������鑬�x

    private bool floating = false;
    private bool falling = false;
    private Vector3 initialPosition;
    private Rigidbody rb;

    void Start()
    {
        initialPosition = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("mashroom") && !floating && !falling)
        {
            floating = true;
            // ��яオ��
            rb.velocity = new Vector3(rb.velocity.x, Mathf.Sqrt(2f * floatingHeight * Mathf.Abs(Physics.gravity.y)), rb.velocity.z);
            Debug.Log("Player collided with obstacle. Floating...");
        }
    }

    void Update()
    {
        if (floating)
        {
            // �㏸��
            if (transform.position.y >= initialPosition.y + floatingHeight)
            {
                floating = false;
                falling = true;
            }
        }

        if (falling)
        {
            // ������
            float newY = Mathf.MoveTowards(transform.position.y, initialPosition.y - fallingHeight, fallingSpeed * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
        if (transform.position.y <= initialPosition.y)
        {
            falling = false;
            // �����I����ɍĂє�ׂ�悤�ɂ���
            floating = false;
        }
    }
}