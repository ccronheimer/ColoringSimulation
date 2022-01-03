using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    // set of images for the game
    public Sprite[] sprites;
    LevelCreator levelCreator;

    // number of cubes drawn
    int numberHit;

    // progress % 
    float progress;

    // amount of Cubes from the image to draw
    int amountOfCubes;

    // UI animator fades
    public Animator animator;

    // Progress text 
    public Text progressText;
    // cubes to draw
    GameObject[] cubes;

   public AudioSource sound;
    // Start is called before the first frame update
    void Start()
    {
        levelCreator = FindObjectOfType<LevelCreator>();
        StartCoroutine(CreateLevel());

    }
    public Sprite[] GetSprites()
    {
        return sprites;
    }

    public void EndGame()
    {
        sound.Play();
        // wave ani
        // cubes jump timed from distance of starting cubes (corner)
        for (int i = 0; i < cubes.Length; i++)
        {
            StartCoroutine(cubes[i].GetComponent<Cube>().JumpUp(Vector3.Distance(cubes[i].transform.position, new Vector3(0, 0, 0))));
        }

        // end game sequence
        StartCoroutine(NextGame());
    }

    IEnumerator NextGame()
    {
        // wait until wave ani is done
        yield return new WaitForSeconds(2f);
        // fade out and fade in new game
        animator.SetBool("Fade", true);

        yield return new WaitForSeconds(0.25f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // called from player script when it hitsa cube
    public void CubeHit()
    {
        numberHit += 1;

        // change text on screen cube count
        progressText.text = numberHit.ToString() + "/" + amountOfCubes.ToString();

        if (numberHit >= amountOfCubes)
        {
            EndGame();
            progressText.text = "Complete!";
        }
    }
    IEnumerator CreateLevel()
    {
        yield return new WaitForSeconds(0.1f);

        // pick a randon sprite name to create the level 
        int random = Random.Range(0, sprites.Length);

        levelCreator.CreateLevel(sprites[random].name);

        // reference all drawable cubes in the scene
        cubes = GameObject.FindGameObjectsWithTag("DrawMe");

        // get the number of drawable cubes 
        amountOfCubes = cubes.Length;

        // default starting text
        progressText.text = "0/" + amountOfCubes.ToString();
    }

}
