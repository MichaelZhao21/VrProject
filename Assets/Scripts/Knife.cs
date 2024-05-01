using System;
using System.Collections;
using System.Collections.Generic;
using EzySlice;
using Photon.Pun;
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
    HashSet<GameObject> hoveredObjects = new();
    public float separation = 0.1f;

    [SerializeField]
    private UnityEvent<string> onSlice;

    // This function actually slices stuff
    // Parts adapted from https://github.com/hugoscurti/mesh-cutter/blob/master/Assets/Scripts/MouseSlice.cs
    void OnCollisionEnter(Collision collision)
    {
        if (slicing || !collision.gameObject.CompareTag("ingredient") || !grabbing || !GetComponent<PhotonView>().IsMine)
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
        var cutForward = transform.forward;
        GameObject pl = DrawPlane(cutForward, contact.point);

        // Slice the object
        GameObject upper = PhotonNetwork.Instantiate("CutObject", collision.transform.position, collision.transform.rotation);
        GameObject lower = PhotonNetwork.Instantiate("CutObject", collision.transform.position, collision.transform.rotation);
        var objs = collision.gameObject.SliceInstantiate(pl.transform.position, pl.transform.up, collision.gameObject.GetComponent<Ingredient>().innerMaterial, upper, lower);

        // If objs is null, the object was not sliced
        if (objs == null)
        {
            slicing = false;
            yield return new WaitForEndOfFrame();
        }
        // If only one object was returned, the object was not sliced
        else if (objs.Length != 2)
        {
            PhotonNetwork.Destroy(upper);
            PhotonNetwork.Destroy(lower);
            slicing = false;
            yield return new WaitForEndOfFrame();
        }
        else if (GetMinDim(objs[0].GetComponent<MeshRenderer>().bounds.size) < collision.gameObject.GetComponent<Ingredient>().minCutDim ||
            GetMinDim(objs[1].GetComponent<MeshRenderer>().bounds.size) < collision.gameObject.GetComponent<Ingredient>().minCutDim)
        {
            Debug.Log("COULDN'T CUT: " + GetMinDim(objs[0].GetComponent<MeshRenderer>().bounds.size) + " " + GetMinDim(objs[1].GetComponent<MeshRenderer>().bounds.size));
            PhotonNetwork.Destroy(objs[0]);
            PhotonNetwork.Destroy(objs[1]);
            slicing = false;
            yield return new WaitForEndOfFrame();
        }
        else
        {
            Debug.Log("CUT: " + GetMinDim(objs[0].GetComponent<MeshRenderer>().bounds.size) + " " + GetMinDim(objs[1].GetComponent<MeshRenderer>().bounds.size));
            // So new objects dont hit it lol
            collision.gameObject.GetComponent<MeshCollider>().enabled = false;

            // Figure out name of new game objects
            string name = collision.gameObject.name;
            if (!name.EndsWith("-cut"))
            {
                name += "-cut";
            }

            // Loop through objs
            foreach (GameObject obj in objs)
            {
                // Add name
                obj.name = name;

                // Add meshcollider
                // obj.GetComponent<MeshCollider>();
                // obj.GetComponent<MeshCollider>().convex = true;

                // Add rigidbody
                // obj.AddComponent<Rigidbody>();

                // Add outline
                // var o = obj.GetComponent<Outline>();
                // o.OutlineMode = Outline.Mode.OutlineAll;
                // o.OutlineColor = Color.cyan;
                // o.OutlineWidth = 4f;
                // o.enabled = false;

                // Add state change
                var sc = obj.GetComponent<StateChange>();
                var currSc = collision.gameObject.GetComponent<StateChange>();
                sc.stateName = currSc.stateName;
                sc.gameMaster = currSc.gameMaster;

                // Add ingredient script
                var ing = obj.GetComponent<Ingredient>();
                var currIng = collision.gameObject.GetComponent<Ingredient>();
                ing.grabbed = currIng.grabbed;
                ing.minCutDim = currIng.minCutDim;
                ing.expectedPieces = currIng.expectedPieces;
                ing.totalCookingTime = currIng.totalCookingTime;
                ing.overcookedTime = currIng.overcookedTime;
                ing.cookingPercent = currIng.cookingPercent;
                ing.cookingState = currIng.cookingState;
                ing.isCooking = currIng.isCooking;
                ing.innerMaterial = currIng.innerMaterial;

                // Add audio source
                // var audSource = obj.AddComponent<AudioSource>();
                // audSource.playOnAwake = false;
                // audSource.clip = collision.gameObject.GetComponent<AudioSource>().clip;

                // Add grab interactable
                // var xrgi = obj.AddComponent<XRGrabInteractable>();
                // var xrgi = obj.GetComponent<XRGrabNetworkInteractable>();
                // xrgi.useDynamicAttach = true;
                // xrgi.hoverEntered.AddListener((hoverEventArgs) => ing.EnableOutline());
                // xrgi.hoverExited.AddListener((hoverEventArgs) => ing.DisableOutline());
                // xrgi.selectEntered.AddListener((selectEventArgs) => ing.Grab());
                // xrgi.selectExited.AddListener((selectEventArgs) => ing.Ungrab());

                // Add grab transformer
                // obj.AddComponent<XRGeneralGrabTransformer>();

                // Set ingredient tag
                // obj.tag = "ingredient";
            }

            var origId = collision.gameObject.GetComponent<PhotonView>().ViewID;
            var upperId = upper.GetComponent<PhotonView>().ViewID;
            var lowerId = lower.GetComponent<PhotonView>().ViewID;

            // Destroy the original object
            Destroy(collision.gameObject);

            // Cut remote call
            GetComponent<PhotonView>().RPC("RemoteCut", RpcTarget.Others, origId, contact.point, cutForward, upperId, lowerId);

            yield return new WaitForSeconds(0.05f);
        }

        slicing = false;
    }

    [PunRPC]
    public void RemoteCut(int origId, Vector3 contactPoint, Vector3 cutForward, int upperId, int lowerId) {
        // Get all game objects
        GameObject orig = PhotonView.Find(origId).gameObject;
        GameObject upper = PhotonView.Find(upperId).gameObject;
        GameObject lower = PhotonView.Find(lowerId).gameObject;

        // Calculate the normal of plane to slice
        GameObject pl = DrawPlane(cutForward, contactPoint);

        // Cut da shi
        var objs = orig.SliceInstantiate(pl.transform.position, pl.transform.up, orig.GetComponent<Ingredient>().innerMaterial, upper, lower);
        Debug.Log("REMOTE CUT: " + GetMinDim(objs[0].GetComponent<MeshRenderer>().bounds.size) + " " + GetMinDim(objs[1].GetComponent<MeshRenderer>().bounds.size));

        // So new objects dont hit it lol
        orig.GetComponent<MeshCollider>().enabled = false;

        // Figure out name of new game objects
        string name = orig.name;
        if (!name.EndsWith("-cut"))
        {
            name += "-cut";
        }

        // Loop through objs
        foreach (GameObject obj in objs)
        {
            // Add name
            obj.name = name;

            // Add state change
            var sc = obj.GetComponent<StateChange>();
            var currSc = orig.GetComponent<StateChange>();
            sc.stateName = currSc.stateName;
            sc.gameMaster = currSc.gameMaster;

            // Add ingredient script
            var ing = obj.GetComponent<Ingredient>();
            var currIng = orig.GetComponent<Ingredient>();
            ing.grabbed = currIng.grabbed;
            ing.minCutDim = currIng.minCutDim;
            ing.expectedPieces = currIng.expectedPieces;
            ing.totalCookingTime = currIng.totalCookingTime;
            ing.overcookedTime = currIng.overcookedTime;
            ing.cookingPercent = currIng.cookingPercent;
            ing.cookingState = currIng.cookingState;
            ing.isCooking = currIng.isCooking;
            ing.innerMaterial = currIng.innerMaterial;
        }

        // Destroy the original object
        Destroy(orig);
    }

    // This function is for the outline effect on hover over a thing to cut
    void Update()
    {
        if (!grabbing)
        {
            return;
        }

        Collider[] colliders = Physics.OverlapCapsule(transform.position + transform.up * 0.5f, transform.position - transform.up * 0.5f, 0.1f);
        HashSet<GameObject> newHoveredObjects = new();

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
        RemoveNulls();
        foreach (GameObject hoveredObject in hoveredObjects)
        {
            if (!newHoveredObjects.Contains(hoveredObject))
            {
                hoveredObject.GetComponent<Outline>().enabled = false;
            }
        }

        hoveredObjects = newHoveredObjects;
    }

    private void RemoveNulls()
    {
        hoveredObjects.RemoveWhere(g => g == null);
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

    private float GetMinDim(Vector3 size)
    {
        return Math.Min(Math.Min(size.x, size.y), size.z);
    }
}
