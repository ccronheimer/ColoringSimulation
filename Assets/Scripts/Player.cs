using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    // gameobject rigidbody 
    Rigidbody rb;

    // crayon sensitivity;
    public float speed = 100f;

    // is mouse down 
    bool mouseDown;

    // gameobject mesh renderer
    MeshRenderer meshRenderer;

    // color we are
    Color colorTo;
    Color colorFrom, colorFrom2;
    
    float elapsedTime, waitTime;

    Manager manager;
    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        manager = FindObjectOfType<Manager>();

        rb = GetComponent<Rigidbody>();
        Debug.Log("test");
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.tag == "DrawMe")
        {
            manager.CubeHit();
            other.gameObject.GetComponent<Cube>().Hit();
            colorTo = other.gameObject.GetComponent<Cube>().GetColor();
            StartCoroutine(FadeColor());
        }
    }

    // cleaner update 
    IEnumerator FadeColor()
    {
        waitTime = 3f; // coroutine wait time
        elapsedTime = 0; // time elapse
        colorFrom = meshRenderer.materials[0].color; // past color
        colorFrom2 = new Color(colorFrom.r * 1.2f, colorFrom.g * 1.2f, colorFrom.b * 1.2f); // past lighter color (paper)

        // fades crayon material to crayonTo (color we are touching currently)
        while (elapsedTime < waitTime)
        {
            // crayon 
            meshRenderer.materials[0].color = Color.Lerp(colorFrom, colorTo, (elapsedTime / waitTime));

            // paper
            meshRenderer.materials[1].color = Color.Lerp(colorFrom2, new Color(colorTo.r * 1.2f, colorTo.g * 1.2f, colorTo.b * 1.2f), (elapsedTime / waitTime));
            
            elapsedTime += Time.deltaTime;
            yield return null;

        } 
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseDown = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            mouseDown = false;
        }

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (mouseDown)
        {
            Vector3 m_Input = new Vector3(Input.GetAxis("Mouse X"), 0, Input.GetAxis("Mouse Y"));
            rb.MovePosition(transform.position + m_Input * Time.deltaTime * speed);
        }

    }
}