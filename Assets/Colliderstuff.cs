using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

public class Colliderstuff : MonoBehaviour
{
    public Material targetMaterial;
    public List<GameObject> targetObjects = new List<GameObject>();//For accessing targetObjects list
     
    // Start is called before the first frame update
    void Start()
    {
        AddMeshCollidersRecursively(transform);
        AddRigidbodyRecursively(transform);

        // Find all GameObjects with the specified material
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            //Material mat1 = renderer.sharedMaterial;
            if (renderer != null)
            {
                if(renderer.material.name == targetMaterial.name + " (Instance)")
                {
                    // Assign the specified tag to the GameObject
                    obj.tag = "Conductor";
                }
            }

            if (obj.CompareTag("Conductor"))
            {
                targetObjects.Add(obj);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void AddMeshCollidersRecursively(Transform parent)
    {
        foreach (Transform child in parent)
        {
            // Check if the child already has a collider
            if (child.GetComponent<Collider>() == null)
            {
                // Add mesh collider to the child object
                MeshCollider meshCollider = child.gameObject.AddComponent<MeshCollider>();
                // Set the mesh collider to be convex if the mesh is convex
                meshCollider.convex = true; // Set to false if you don't want convex collider 
            }

            // Recursively call this function for all children
            AddMeshCollidersRecursively(child);
        }
    }
    public void AddRigidbodyRecursively(Transform parent)
    {
        foreach (Transform child in parent)
                {
            Rigidbody rb = child.GetComponent<Rigidbody>();
            // Check if the child already has a Rigidbody component
            if (child.GetComponent<Rigidbody>() == null)
            {
                // Add Rigidbody to the child object
                rb = child.gameObject.AddComponent<Rigidbody>();
                rb.constraints = RigidbodyConstraints.FreezeAll;
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;//new
            }

            // Recursively call this function for all children
            AddRigidbodyRecursively(child);
        }
    }
}
