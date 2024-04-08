using System.Collections;
using System.Collections.Generic;
using EzySlice;
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

        // Calculate the normal of plane to slice
        Debug.DrawRay(contact.point, transform.forward, Color.red, 10.0f);

        GameObject pl = DrawPlane(transform.forward, contact.point);

        // Slice the object
        var objs = collision.gameObject.SliceInstantiate(pl.transform.position, pl.transform.up, collision.gameObject.GetComponent<MeshRenderer>().material);

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
        else
        {
            // So new objects dont hit it lol
            collision.gameObject.GetComponent<MeshCollider>().enabled = false;

            // Place objects back where they should go
            GameObject upperHull = objs[0];
            GameObject lowerHull = objs[1];

            upperHull.transform.position = collision.gameObject.transform.position;
            lowerHull.transform.position = collision.gameObject.transform.position;

            // Loop through objs
            foreach (GameObject obj in objs)
            {
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
                obj.AddComponent<Outline>();
                var o = obj.GetComponent<Outline>();
                o.OutlineMode = Outline.Mode.OutlineAll;
                o.OutlineColor = Color.cyan;
                o.OutlineWidth = 4f;
                o.enabled = false;

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
        plane.GetComponent<MeshCollider>().enabled = false;
        plane.SetActive(false);
        return plane;
    }
}
