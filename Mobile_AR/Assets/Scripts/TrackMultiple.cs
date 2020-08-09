using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARTrackedImageManager))]
public class TrackMultiple : MonoBehaviour
{
    [Header("Objects in scene to be spawned. Amount must match the number of images in Reference Image Library")]
    public List<GameObject> ObjectsToPlace;

    private Dictionary<string, GameObject> objectDictionary = new Dictionary<string, GameObject>();
    private ARTrackedImageManager manager;

    //link each image in reference library to the respective objects to place, then activate for now
    private void Awake()
    {
        manager = GetComponent<ARTrackedImageManager>();
    }

    private void Start()
    {
        for (int i = 0; i < manager.referenceLibrary.count; i++)
        {
            objectDictionary.Add(manager.referenceLibrary[i].name, ObjectsToPlace[i]);
            ObjectsToPlace[i].SetActive(false);
        }
    }

    //set up to call "OnImageChanged()" when the "trackedImagesChanged" event is called from the "ARTrackedImageManager" component. 
    private void OnEnable()
    {
        manager.trackedImagesChanged += OnImageChanged;
    }
    private void OnDisable()
    {
        manager.trackedImagesChanged -= OnImageChanged;
    }
    
    public void OnImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        //foreach known image that has been updated, if tracking was lost then deactivate, else move to new position/rotation, and reactivate if tracking was found again
        foreach (ARTrackedImage updated in eventArgs.updated)
        {
            //TODO: SET TIMER TO SEE HOW LONG TRACKING HAS BEEN LIMITED
            if (updated.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.None || updated.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Limited)
            {
                objectDictionary[updated.referenceImage.name].SetActive(false);
            }
            else
            {
                if (!objectDictionary[updated.referenceImage.name].activeSelf)
                {
                    objectDictionary[updated.referenceImage.name].SetActive(true);
                }

                objectDictionary[updated.referenceImage.name].transform.position = updated.transform.position;
                objectDictionary[updated.referenceImage.name].transform.rotation = updated.transform.rotation;
            }
        }

        //foreach image found, look it up in the dictionary and enable it 
        foreach (ARTrackedImage addedImage in eventArgs.added)
        {
            objectDictionary[addedImage.referenceImage.name].SetActive(true);
        }

        //foreach image lost, look it up in the dictionary and remove it SHOULD WORK BUT APPARENTLY DOESNT IN ANDROID ATM
        foreach (ARTrackedImage removed in eventArgs.removed)
        {
            objectDictionary[removed.referenceImage.name].SetActive(false);
        }

    }


    //Methods to Pause and Restart Videos 
    //private void PlayVideos(GameObject rootObject)
    //{
    //    VideoPlayer[] videoPlayers = rootObject.GetComponentsInChildren<VideoPlayer>();

    //    foreach(VideoPlayer player in videoPlayers)
    //    {
    //        player.Play();
    //    }

    //}

    //private void PauseVideos(GameObject rootObject)
    //{
    //    VideoPlayer[] videoPlayers = rootObject.GetComponentsInChildren<VideoPlayer>();

    //    foreach (VideoPlayer player in videoPlayers)
    //    {
    //        player.Pause();
    //    }

    //}
}
