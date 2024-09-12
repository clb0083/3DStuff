/*AU Team 1 (SP & SU 2024) used the help of Auburn graduate student Jake Botello
to learn Unity and C# in integrating design ideas. The team applied background
knowledge of MATLAB and C++ coding languages to develop various tools and
functions used throughout this script. ChatGPT was also used as a troubleshooting
reference.*/

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
/*using UnityEditor.Callbacks;*/
using UnityEngine;
using TMPro;
using System.Linq;
using JetBrains.Annotations;

//This script sets up the circuit board in order to be simulated.
/*It does this by applying colliders to the base components of the board for physical interations,
and then copies the colliders to an empty object which is placed slightly above the original object
in order to act as the trigger which actually allows the whiskers to tell if a bridge has occured.*/
public class TriggerControl : MonoBehaviour
{
    public List<GameObject> targetObjects = new List<GameObject>();
    public Rigidbody rb;
    public Vector3 offset = new Vector3(0, 0, 0.005f);
    public int maxDepth = 5; 
    public Vector3 newScale = new Vector3(10, 10, 10);
    public TMP_InputField num_mat;
    public string material_text;
    public int num_mat_int;
    public Material triggerMaterial;
    public TMP_InputField material_input;
    public List<GameObject> objectvalues_ = new List<GameObject>();
    public List<GameObject> testerfill = new List<GameObject>();
    
    //Accesses the circuit board and applys proper settings in order for the simulation to run.
    //Additionlly, this runs the scripts that applys colliders and triggers to the components of the board.
    void Start()
    {   
        gameObject.tag ="CircuitBoard";
        
        Transform topTransform = transform.Find("Top");
        if (topTransform != null)
        {
            Destroy(topTransform.gameObject);
        }
        
        rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        transform.localScale = newScale;
        transform.position = Vector3.zero;

        AddMeshCollidersRecursively(transform);
        StartCoroutine(CopyMeshCollidersToEmptyObjects(transform, 0));

        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        material_input.onValueChanged.AddListener(delegate { checkifconduct(testerfill, objectvalues_, material_input.text); } );
    }

    // Update is called once per frame
    void Update()
    {
    }

    //Applys colliders to the child components on the board.
    public void AddMeshCollidersRecursively(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (child.GetComponent<Collider>() == null)
            {
                MeshCollider meshCollider = child.gameObject.AddComponent<MeshCollider>();
                meshCollider.convex = true;
            }
            AddMeshCollidersRecursively(child);
        }
    }
    //Copies the colliders to the copied objects, which are triggers for the whiskers
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
                if (child == null || child.GetComponent<MeshCollider>() == null)
                {
                    continue;
                }

                MeshCollider originalMeshCollider = child.GetComponent<MeshCollider>();
                if (originalMeshCollider != null)
                {
                    testerfill.Add(child.gameObject);
                    GameObject emptyObject = new GameObject(child.name + "_ColliderCopy");

                    emptyObject.transform.position = child.TransformPoint(offset);
                    emptyObject.transform.rotation = child.rotation;
                    emptyObject.transform.localScale = child.lossyScale;

                    MeshCollider newMeshCollider = emptyObject.AddComponent<MeshCollider>();
                    newMeshCollider.sharedMesh = originalMeshCollider.sharedMesh;
                    newMeshCollider.convex = originalMeshCollider.convex;
                    newMeshCollider.isTrigger = true;

                    emptyObject.transform.SetParent(parent, true);

                    Renderer renderer = emptyObject.AddComponent<MeshRenderer>();
                    MeshFilter meshFilter = emptyObject.AddComponent<MeshFilter>();
                    meshFilter.mesh = originalMeshCollider.sharedMesh;
                    renderer.material = new Material(triggerMaterial);

                    emptyObject.AddComponent<TriggerTracker>();
                    objectvalues_.Add(emptyObject);
                }

                yield return StartCoroutine(CopyMeshCollidersToEmptyObjects(child, depth + 1));
            }
        }
    //Checks if object is a conductor by comparing materials to the open given by the users. 
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
}
