using System.Collections;
using System.Collections.Generic;
using EzySlice;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

public class Knife : MonoBehaviour
{
    bool slicing = false;
    bool grabbing = false;
    List<GameObject> hoveredObjects = new();
    public float separation = 0.1f;

    [SerializeField]
    private UnityEvent<string> onSlice;

    // This function actually slices stuff
    // Parts adapted from https://github.com/hugoscurti/mesh-cutter/blob/master/Assets/Scripts/MouseSlice.cs
    void OnCollisionEnter(Collision collision)
    {
        if (slicing || !collision.gameObject.CompareTag("ingredient"))
        {
            return;
        }

        slicing = true;

        onSlice.Invoke(collision.gameObject.name);

        StartCoroutine(SliceObject(collision));
    }

    IEnumerator SliceObject(Collision collision)
    {
        ContactPoint contact = collision.GetContact(0);

        // Used for debugging!
        // Debug.DrawRay(contact.point, transform.forward, Color.red, 10.0f);

        // Calculate the normal of plane to slice
        GameObject pl = DrawPlane(transform.forward, contact.point);

        // Slice the object
        var objs = collision.gameObject.SliceInstantiate(pl.transform.position, pl.transform.up, collision.gameObject.GetComponent<Ingredient>().innerMaterial);

        // If objs is null, the object was not sliced
        if (objs == null)
        {
            slicing = false;
            yield return new WaitForEndOfFrame();
        }
        // If only one object was returned, the object was not sliced
        else if (objs.Length != 2)
        {
            Destroy(objs[0]);
            slicing = false;
            yield return new WaitForEndOfFrame();
        }
        else if (GetVolume(objs[0].GetComponent<MeshRenderer>().bounds.size) < collision.gameObject.GetComponent<Ingredient>().minVolume ||
            GetVolume(objs[1].GetComponent<MeshRenderer>().bounds.size) < collision.gameObject.GetComponent<Ingredient>().minVolume)
        {
            Debug.Log("COULDN'T CUT: " + GetVolume(objs[0].GetComponent<MeshRenderer>().bounds.size) + " " + GetVolume(objs[1].GetComponent<MeshRenderer>().bounds.size));
            Destroy(objs[0]);
            Destroy(objs[1]);
            slicing = false;
            yield return new WaitForEndOfFrame();
        }
        else
        {
            Debug.Log("CUT: " + GetVolume(objs[0].GetComponent<MeshRenderer>().bounds.size) + " " + GetVolume(objs[1].GetComponent<MeshRenderer>().bounds.size));
            // So new objects dont hit it lol
            collision.gameObject.GetComponent<MeshCollider>().enabled = false;

            // Figure out name of new game objects
            string name = collision.gameObject.name;
            if (!name.EndsWith("-cut"))
            {
                name += "-cut";
            }

            // Place objects back where they should go
            GameObject upperHull = objs[0];
            GameObject lowerHull = objs[1];

            upperHull.transform.position = collision.gameObject.transform.position;
            lowerHull.transform.position = collision.gameObject.transform.position;

            // Loop through objs
            foreach (GameObject obj in objs)
            {
                // Add name
                obj.name = name;

                // Add meshcollider
                obj.AddComponent<MeshCollider>();
                obj.GetComponent<MeshCollider>().convex = true;

                // Add rigidbody
                obj.AddComponent<Rigidbody>();

                // Add grab interactable
                obj.AddComponent<XRGrabInteractable>();

                // Add grab transformer
                obj.AddComponent<XRGeneralGrabTransformer>();

                // Add outline
                var o = obj.AddComponent<Outline>();
                o.OutlineMode = Outline.Mode.OutlineAll;
                o.OutlineColor = Color.cyan;
                o.OutlineWidth = 4f;
                o.enabled = false;

                // Add state change
                var sc = obj.AddComponent<StateChange>();
                var currSc = collision.gameObject.GetComponent<StateChange>();
                sc.stateName = currSc.stateName;
                sc.gameMaster = currSc.gameMaster;

                // Add ingredient script
                var ing = obj.AddComponent<Ingredient>();
                var currIng = collision.gameObject.GetComponent<Ingredient>();
                ing.grabbed = currIng.grabbed;
                ing.minVolume = currIng.minVolume;
                ing.expectedPieces = currIng.expectedPieces;
                ing.totalCookingTime = currIng.totalCookingTime;
                ing.overcookedTime = currIng.overcookedTime;
                ing.cookingPercent = currIng.cookingPercent;
                ing.cookingState = currIng.cookingState;
                ing.isCooking = currIng.isCooking;
                ing.innerMaterial = currIng.innerMaterial;

                // Add audio source
                var audSource = obj.AddComponent<AudioSource>();
                audSource.playOnAwake = false;
                audSource.clip = collision.gameObject.GetComponent<AudioSource>().clip;

                // Set ingredient tag
                obj.tag = "ingredient";
            }

            // Destroy the original object
            Destroy(collision.gameObject);

            yield return new WaitForSeconds(0.05f);
        }

        slicing = false;
    }

    // This function is for the outline effect on hover over a thing to cut
    void Update()
    {
        if (!grabbing)
        {
            return;
        }

        Collider[] colliders = Physics.OverlapCapsule(transform.position + transform.up * 0.5f, transform.position - transform.up * 0.5f, 0.1f);
        List<GameObject> newHoveredObjects = new();

        // Enable outlines for all hovered objects
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("ingredient"))
            {
                newHoveredObjects.Add(collider.gameObject);
                collider.gameObject.GetComponent<Outline>().enabled = true;
            }
        }

        // Disable all outlines that are not hovered anymore
        foreach (GameObject hoveredObject in hoveredObjects)
        {
            if (!newHoveredObjects.Contains(hoveredObject))
            {
                hoveredObject.GetComponent<Outline>().enabled = false;
            }
        }

        hoveredObjects = newHoveredObjects;
    }

    // Need to assign this to the Select Entered event of the Grabbable script
    public void OnGrab()
    {
        grabbing = true;
    }

    // Need to assign this to the Select Exited event of the Grabbable script
    public void OnStopGrab()
    {
        grabbing = false;
    }

    // Adapted from https://github.com/hugoscurti/mesh-cutter/blob/01f32c4ee8dd025c257b281d4f34fa5df0dd2303/Assets/Scripts/MouseSlice.cs#L24C5-L31C6
    GameObject DrawPlane(Vector3 normalVec, Vector3 point)
    {
        // Destroy all previous planes
        GameObject[] planes = GameObject.FindGameObjectsWithTag("plane");
        foreach (GameObject p in planes)
        {
            Destroy(p);
        }

        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        Quaternion rotate = Quaternion.FromToRotation(Vector3.up, normalVec);

        plane.transform.localRotation = rotate;
        plane.transform.position = point;
        plane.tag = "plane";
        plane.name = "Cut Plane";
        plane.GetComponent<MeshCollider>().enabled = false;
        plane.GetComponent<MeshRenderer>().enabled = false;
        return plane;
    }

    private float GetVolume(Vector3 size)
    {
        return size.x * size.y * size.z;
    }
}
