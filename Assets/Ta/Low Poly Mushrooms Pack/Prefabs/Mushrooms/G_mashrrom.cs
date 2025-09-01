using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_mashroom : MonoBehaviour
{
    public float floatingHeight = 500f; // ”ò‚Ñã‚ª‚é‚‚³
    public float floatingSpeed = 3f; // ”ò‚Ñã‚ª‚é‘¬“x
    public float fallingHeight = 1000f; // —‰º‚·‚é‚‚³
    public float fallingSpeed = 3f; // —‰º‚·‚é‘¬“x

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
            // ”ò‚Ñã‚ª‚é
            rb.velocity = new Vector3(rb.velocity.x, Mathf.Sqrt(2f * floatingHeight * Mathf.Abs(Physics.gravity.y)), rb.velocity.z);
            Debug.Log("Player collided with obstacle. Floating...");
        }
    }

    void Update()
    {
        if (floating)
        {
            // ã¸’†
            if (transform.position.y >= initialPosition.y + floatingHeight)
            {
                floating = false;
                falling = true;
            }
        }

        if (falling)
        {
            // —‰º’†
            float newY = Mathf.MoveTowards(transform.position.y, initialPosition.y - fallingHeight, fallingSpeed * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
        if (transform.position.y <= initialPosition.y)
        {
            falling = false;
            // —‰ºI—¹Œã‚ÉÄ‚Ñ”ò‚×‚é‚æ‚¤‚É‚·‚é
            floating = false;
        }
    }
}