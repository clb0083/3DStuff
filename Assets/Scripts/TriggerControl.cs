using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;

public class TriggerControl : MonoBehaviour
{
    public Material targetMaterial;
    public List<GameObject> targetObjects = new List<GameObject>();//For accessing targetObjects list
    public Rigidbody rb;
    public Vector3 offset = new Vector3(0, 0, 0.005f); // Offset to place the new objects slightly above the originals
    public int maxDepth = 5; // Maximum depth of recursion to avoid freezing
    public Vector3 newScale = new Vector3(10, 10, 10);

 
    void Start()
    {   
        // Setting whole board as rigidbody + fix settings  
        rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        transform.localScale = newScale;

        // Adding Colliders to all objects
        AddMeshCollidersRecursively(transform);
        StartCoroutine(CopyMeshCollidersToEmptyObjects(transform, 0));

        //Find all GameObjects with the specified material
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        // Find all GameObjects with the specified material
        foreach (GameObject obj in allObjects)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null && renderer.material.name == targetMaterial.name + " (Instance)")
            {
                // Assign the specified tag to the GameObject
                obj.tag = "Conductor";
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
            meshCollider.convex = true;
        }

        // Recursively call this function for all children
        AddMeshCollidersRecursively(child);
    }
}
IEnumerator CopyMeshCollidersToEmptyObjects(Transform parent, int depth)
    {
        if (depth > maxDepth)
        {
            yield break;
        }

        List<Transform> children = new List<Transform>();
        foreach (Transform child in parent)
        {
            children.Add(child);
        }

        foreach (Transform child in children)
        {
            // Check if the child has a Mesh Collider
            MeshCollider originalMeshCollider = child.GetComponent<MeshCollider>();
            if (originalMeshCollider != null)
            {
                // Create an empty GameObject
                GameObject emptyObject = new GameObject(child.name + "_ColliderCopy");

                // Set position, rotation, and scale using the original's world space
                emptyObject.transform.position = child.TransformPoint(offset);
                emptyObject.transform.rotation = child.rotation;
                emptyObject.transform.localScale = child.lossyScale;

                // Copy the Mesh Collider to the new GameObject
                MeshCollider newMeshCollider = emptyObject.AddComponent<MeshCollider>();
                newMeshCollider.sharedMesh = originalMeshCollider.sharedMesh;
                newMeshCollider.convex = originalMeshCollider.convex;
                newMeshCollider.isTrigger = true;

                // Set the new GameObject as a child of the parent
                emptyObject.transform.SetParent(parent, true);

                // Assign "Conductor" tag if the original child has it
                if (child.CompareTag("Conductor"))
                {
                    emptyObject.tag = "Conductor";
                }
            }

            // Recursively call this function for all children
            yield return StartCoroutine(CopyMeshCollidersToEmptyObjects(child, depth + 1));
        }
    }
//ORIG
 /*IEnumerator CopyMeshCollidersToEmptyObjects(Transform parent, int depth)
    {
        if (depth > maxDepth)
        {
            yield break;
        }

        List<Transform> children = new List<Transform>();
        foreach (Transform child in parent)
        {
            children.Add(child);
        }

        foreach (Transform child in children)
        {
            // Check if the child has a Mesh Collider
            MeshCollider originalMeshCollider = child.GetComponent<MeshCollider>();
            if (originalMeshCollider != null)
            {
                // Create an empty GameObject
                GameObject emptyObject = new GameObject(child.name + "_ColliderCopy");
                emptyObject.transform.position = child.position + offset;
                emptyObject.transform.rotation = child.rotation;
                emptyObject.transform.localScale = child.localScale;
                
                // Copy the Mesh Collider to the new GameObject
                MeshCollider newMeshCollider = emptyObject.AddComponent<MeshCollider>();
                newMeshCollider.sharedMesh = originalMeshCollider.sharedMesh;
                newMeshCollider.convex = originalMeshCollider.convex;
                newMeshCollider.isTrigger = true;

                // Set the new GameObject as a child of the parent
                emptyObject.transform.SetParent(parent);
                // Assign "Conductor" tag if the original child has it
                if (child.CompareTag("Conductor"))
                {
                    emptyObject.tag = "Conductor";
                }
            }

            // Recursively call this function for all children
            yield return StartCoroutine(CopyMeshCollidersToEmptyObjects(child, depth + 1));
        }
    }*/
}
