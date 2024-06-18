using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

public class Colliderstuff : MonoBehaviour
{
    public Material targetMaterial;
    public List<GameObject> targetObjects = new List<GameObject>();//For accessing targetObjects list
    public Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {   
        //setting whole board as rigidbody + fix settings
        rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation; //| RigidbodyConstraints.FreezePositionY;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        //Adding Colliders to all objects
        AddMeshCollidersRecursively(transform);   

        //Find all GameObjects with the specified material
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
                    MeshCollider meshTriggerCollider = obj.GetComponent<MeshCollider>();
                    if (meshTriggerCollider != null)
                    {
                    meshTriggerCollider = obj.AddComponent<MeshCollider>();
                    meshTriggerCollider.convex = true;
                    meshTriggerCollider.isTrigger = true;
                    }
                    
                }
            }

            if (obj.CompareTag("Conductor"))
            {
                targetObjects.Add(obj);
                //Collider triggerCollider = obj.AddComponent<BoxCollider>(); //MeshCollider triggerCollider = obj.AddComponent<BoxComponent>();
                //print("hey");
                //triggerCollider.convex = true;
                //triggerCollider.isTrigger = true;
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
}
