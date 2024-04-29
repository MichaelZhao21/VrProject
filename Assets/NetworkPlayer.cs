using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Photon.Pun;

public class NetworkPlayer : MonoBehaviour
{
    public Transform cook;
    private PhotonView photonView;

    public GameObject cam;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if(photonView.IsMine) 
        {
            cook.gameObject.SetActive(false);

            // MapPosition(cook, XRNode.Head);

            cam = GameObject.Find("Main Camera");
            cook.transform.position = new Vector3(cam.transform.position.x, 0f, cam.transform.position.z);
            cook.transform.rotation = new Quaternion(0f, cam.transform.rotation.y, 0.0f, cam.transform.rotation.w);
            //cook.transform.SetParent(cam.transform);

        }
        
    }

    void MapPosition(Transform target, XRNode node)
    {
        InputDevices.GetDeviceAtXRNode(node).TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 position);
        InputDevices.GetDeviceAtXRNode(node).TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rotation);
        Debug.Log(position);
        target.position = position;
        target.rotation = rotation;
    }
}
