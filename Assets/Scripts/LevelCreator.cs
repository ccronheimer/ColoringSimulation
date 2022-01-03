using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCreator : MonoBehaviour
{
    // convert sprite to texture so we can read easier
    Texture2D imageToRead;

    // all the cubes
    private GameObject[] imageWall;

    // set of images from Manager
    Sprite[] images;

    // if we see transparent (which is black) color cut it out from output
    public Color deleteColor;

    // flat mesh for the border defualt is rounded
    public Mesh FlatCube;

    // what we want to control all cubes with 
    public GameObject parent;

    // border material
    public Material borderMat;

    // used for optimization 1 material for many colours
    MaterialPropertyBlock _propBlock;

    [System.Serializable]
    public class Pool
    {

        public string tag;  // add a tag to your go
        public GameObject prefab; // the object you want to spawn for the map
        public int size; // size of image your reading so 10x10 = 100
    }

    // list of pools 
    public List<Pool> pools;
    Manager manager;

    // dictionary of pools
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    void Start()
    {
        manager = FindObjectOfType<Manager>();
        images = manager.GetSprites();
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        // optimization so we done have many instances of materials 
        // we can use the same Cube material but change color for each cube
        _propBlock = new MaterialPropertyBlock();

        // create a pool of 32x32=1024 cubes that are instantiated at once 
        // improves runtime 
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);

            }

            poolDictionary.Add(pool.tag, objectPool);

        }
    }

    public void SpawnFromPool(string tag, Vector3 position, Color color, bool border)
    {

        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.Log("Pool doesnt exist");
            return;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;

        // parent to the object we are on so we can move everything at once
        objectToSpawn.transform.SetParent(parent.transform);

        // drawable
        if (!border)
        {
            objectToSpawn.GetComponent<MeshRenderer>().GetPropertyBlock(_propBlock);

            // set cube colour
            _propBlock.SetColor("_Color", color);
            objectToSpawn.GetComponent<Cube>().SetColor(color);
            objectToSpawn.GetComponent<MeshRenderer>().SetPropertyBlock(_propBlock);

            objectToSpawn.GetComponent<MeshRenderer>().enabled = false;
            objectToSpawn.tag = "DrawMe";

        }
        else // else is not drawable 
        {
            objectToSpawn.GetComponent<MeshFilter>().mesh = FlatCube;
            // cube colour to be white since border
            objectToSpawn.GetComponent<MeshRenderer>().material = borderMat;
            objectToSpawn.tag = "Border";

            // remove useless components for border cubes
            Destroy(objectToSpawn.GetComponent<BoxCollider>());
            Destroy(objectToSpawn.GetComponent<Cube>());
            Destroy(objectToSpawn.GetComponent<AudioSource>());
        }

        poolDictionary[tag].Enqueue(objectToSpawn);
    }

    public void CreateLevel(string name)
    {
        // Debug.Log("Searching For: " + name);

        for (int i = 0; i < images.Length; i++)
        {
            if (images[i].name == name)
            {
                //Debug.Log("size trec: " + (int)pictures[i].textureRect.width + " " + (int)pictures[i].textureRect.height);
                // Debug.Log("size trec: " + (int)pictures[i].rect.width + " " + (int)pictures[i].rect.height);

                // get each pixel from the image texture
                Texture2D croppedTexture = new Texture2D((int)images[i].rect.width, (int)images[i].rect.height);
                var pixels = images[i].texture.GetPixels((int)images[i].rect.x,
                                                        (int)images[i].rect.y,
                                                        (int)images[i].rect.width,
                                                        (int)images[i].rect.height);
                croppedTexture.SetPixels(pixels);
                croppedTexture.Apply();

                imageToRead = croppedTexture;

                // 3d output from our 2d image using our pool of cubes
                createImage();

                break;
            }
        }
    }


    void createImage()
    {

        // get all the pixels colours from the sprite
        Color[] pixels = imageToRead.GetPixels();

        imageWall = new GameObject[pixels.Length];

        // position cubes accordingly 
        for (int i = 0; i < pixels.Length; i++)
        {
            // border
            if (pixels[i] == deleteColor)
            {
                SpawnFromPool("Cube", new Vector3(i % imageToRead.width, 0.01f, i / imageToRead.width), pixels[i], true);

            }
            else
            {
                SpawnFromPool("Cube", new Vector3(i % imageToRead.width, 1f, i / imageToRead.width), pixels[i], false);

            }

        }

    }

}