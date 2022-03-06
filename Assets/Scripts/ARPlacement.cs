using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlacement : MonoBehaviour
{
    private GameObject SpawnObject;
    public GameObject ObjectToSpawn;
    public GameObject PlacementIndicator;
    private Pose PlacementPose;
    private ARRaycastManager araycastmgr;
    private bool placmentPoseIsValid = false;

    void Start()
    {
        araycastmgr = FindObjectOfType<ARRaycastManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(SpawnObject == null && placmentPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began){
            ARObjectPlacement();
        }
        else if(SpawnObject != null && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began){
            OnDestroy();
        }
        UpdatePlacementPose();
        UpdatePlacementIndicator();
   
    }

    void UpdatePlacementIndicator(){
         
         if(SpawnObject == null  && placmentPoseIsValid){
             PlacementIndicator.SetActive(true);
             PlacementIndicator.transform.SetPositionAndRotation(PlacementPose.position,PlacementPose.rotation);
         }
         else{
             PlacementIndicator.SetActive(false);
         }
    }

    void UpdatePlacementPose(){
        var  ScreenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f,0.5f));
        var hits = new List<ARRaycastHit>();
        araycastmgr.Raycast(ScreenCenter,hits,TrackableType.Planes);

        placmentPoseIsValid = hits.Count > 0;
        if(placmentPoseIsValid){
            PlacementPose = hits[0].pose;
            // Inorder to move the arrow of placement indicator we need to have the below  lines of code 
            var CameraForward = Camera.current.transform.forward;
            var CameraBearing = new Vector3(CameraForward.x,0,CameraForward.z).normalized;
            PlacementPose.rotation = Quaternion.LookRotation(CameraBearing);
        }
    }

    void ARObjectPlacement(){
        SpawnObject = Instantiate(ObjectToSpawn,PlacementPose.position,PlacementPose.rotation);
    }
    private void OnDestroy() {
        Destroy(SpawnObject);
    }
}
