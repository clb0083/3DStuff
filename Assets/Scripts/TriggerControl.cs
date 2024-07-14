using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;
using TMPro;
using System.Linq;
using JetBrains.Annotations;

public class TriggerControl : MonoBehaviour
{
    // public Material targetMaterial;
    public List<GameObject> targetObjects = new List<GameObject>();//For accessing targetObjects list
    public Rigidbody rb;
    public Vector3 offset = new Vector3(0, 0, 0.005f); // Offset to place the new objects slightly above the originals
    public int maxDepth = 5; // Maximum depth of recursion to avoid freezing
    public Vector3 newScale = new Vector3(10, 10, 10);
    public TMP_InputField num_mat; //new variable
    public string material_text;
    public int num_mat_int;
    public Material triggerMaterial;
    public TMP_InputField material_input;
    public List<GameObject> objectvalues_ = new List<GameObject>();
    public List<GameObject> testerfill = new List<GameObject>();
    
 
    void Start()
    {   
        gameObject.tag ="CircuitBoard";
        
        Transform topTransform = transform.Find("Top");
        if (topTransform != null)
        {
            Destroy(topTransform.gameObject);
        }
        // Setting whole board as rigidbody + fix settings  
        rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        transform.localScale = newScale;
        transform.position = Vector3.zero;

        // Adding Colliders to all objects
        AddMeshCollidersRecursively(transform);
        StartCoroutine(CopyMeshCollidersToEmptyObjects(transform, 0));

       // material_dict = new Dictionary<string, Material>();

       GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
       

       
     
      material_input.onValueChanged.AddListener(delegate { checkifconduct(testerfill, objectvalues_, material_input.text); } );
        
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
                testerfill.Add(child.gameObject);
                
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

              

                // Add a visual
                Renderer renderer = emptyObject.AddComponent<MeshRenderer>();
                MeshFilter meshFilter = emptyObject.AddComponent<MeshFilter>();
                meshFilter.mesh = originalMeshCollider.sharedMesh;
                renderer.material = new Material(triggerMaterial);

                //add heatmap tracker script
                emptyObject.AddComponent<TriggerTracker>();
                
                objectvalues_.Add(emptyObject);
            }
            // Recursively call this function for all children
            yield return StartCoroutine(CopyMeshCollidersToEmptyObjects(child, depth + 1));
        }
    }

public void checkifconduct(List<GameObject> values, List<GameObject> emptylist, string text_value)
{
    Renderer[] allRenderers = FindObjectsOfType<Renderer>();
    foreach (Renderer renderer in allRenderers)
    {
        foreach (Material material in renderer.materials)
        {
            string materialName = material.name.Replace(" (Instance)", "");
            if (materialName.Equals(text_value, System.StringComparison.OrdinalIgnoreCase))
            {
                GameObject obj = renderer.gameObject;
                obj.tag = "Conductor";
                for (int i = 0; i < emptylist.Count; i++)
                {
                    if (emptylist[i].name.StartsWith(obj.name))
                    {
                        emptylist[i].tag = "ConductorTrigger";
                        break;
                    }
                }
                break;
            }
        }
    }
}




/*public void checkifconduct(List<GameObject> values, List<GameObject> emptylist, string text_value)
{
    //int value_index = 0;
    int overall_val = emptylist.Count;
    Debug.Log("Value");

    for(int i = 0; i < overall_val; i++)
    {     
        Renderer renderer = values[i].GetComponent<Renderer>();//orignial
        //Renderer[] allrenderers = FindObjectsOfType<Renderer>();//new
        //foreach(Renderer renderer in allrenderers)//new
        //{//new
            if (renderer != null && renderer.material.name == (text_value + " (Instance)") && material_input.text != "")         
            {       
                values[i].tag = "Conductor";
                        
                emptylist[i].tag = "ConductorTrigger";           
            }
        //} //new
    }
}*/

}
//ORIGINAL
/*using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;
using TMPro;
using System.Linq;
using JetBrains.Annotations;

public class TriggerControl : MonoBehaviour
{
    public Material targetMaterial;
    public Material triggerMaterial;
    public List<GameObject> targetObjects = new List<GameObject>();//For accessing targetObjects list
    public Rigidbody rb;
    public Vector3 offset = new Vector3(0, 0, 0.005f); // Offset to place the new objects slightly above the originals
    public int maxDepth = 5; // Maximum depth of recursion to avoid freezing
    public Vector3 newScale = new Vector3(10, 10, 10);
    public Color[] colors;

 
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
                    emptyObject.tag = "ConductorTrigger";
                }

                // Add a visual
                Renderer renderer = emptyObject.AddComponent<MeshRenderer>();
                MeshFilter meshFilter = emptyObject.AddComponent<MeshFilter>();
                meshFilter.mesh = originalMeshCollider.sharedMesh;
                renderer.material = new Material(triggerMaterial);

                //add heatmap tracker script
                emptyObject.AddComponent<TriggerTracker>();
                
            }

            // Recursively call this function for all children
            yield return StartCoroutine(CopyMeshCollidersToEmptyObjects(child, depth + 1));
        }
    }
}*/
