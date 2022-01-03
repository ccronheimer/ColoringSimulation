using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    // floor prefab
    public GameObject floor;
    
    // jump animation    
    Animation ani;
    BoxCollider boxCollider;

    int jumpHeight = 5;
    Color color;
    
    public AudioSource sound;
    // Start is called before the first frame update
    void Start()
    {
        ani = GetComponent<Animation>();
        boxCollider = GetComponent<BoxCollider>();
      
        // create a grey flooring under each drawable cube
        if(gameObject.tag == "DrawMe") {
            Instantiate(floor, new Vector3(gameObject.transform.position.x, transform.position.y - 1.39f, transform.position.z), Quaternion.identity);
        }
    }

    public IEnumerator JumpUp(float distance) {
        
        yield return new WaitForSeconds(distance / 50f); // distance from starting cube

        float waitTime = .15f;
        float elapsedTime = 0; 
        Vector3 fromPosition = transform.position;
        Vector3 toPosition = new Vector3(transform.position.x,transform.position.y + jumpHeight,transform.position.z);

        while (elapsedTime < waitTime)
        {
            // smoothly move cube to jumping height
            transform.position = Vector3.Lerp(fromPosition, toPosition, (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        } 

        // lock position to where it is supposed to be
        transform.position = toPosition;

        // jump down right after jumping up
        StartCoroutine(JumpDown()); 
    }

     public IEnumerator JumpDown() {

        yield return new WaitForSeconds(0.5f); // hang in the air time

        float waitTime = .15f;
        float elapsedTime = 0; 
        Vector3 fromPosition = transform.position;

        // smoothly move cube down from jumping height
        Vector3 toPosition = new Vector3(transform.position.x,transform.position.y - jumpHeight,transform.position.z);

        while (elapsedTime < waitTime)
        {
            transform.position = Vector3.Lerp(fromPosition, toPosition, (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        } 

        // lock position to where it is supposed to be
        transform.position = toPosition;
    }
    public void SetColor(Color color){ 
        this.color = color;
    }
    public Color GetColor() {
        return color;
    }
    public void Hit() {

        // show cube
        gameObject.GetComponent<MeshRenderer>().enabled = true;

        // cube grow animation
        ani.Play();

        // delete box collider so no re hits
        boxCollider.enabled = false; 

        // sound 
        sound.Play();
    }
   
}
