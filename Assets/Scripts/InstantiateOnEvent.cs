using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class InstantiateOnEvent : MonoBehaviour
{
    public GameObject objectToInstantiate;
    public Transform instantiationTransform;
    [Tooltip("If set to 0 it will 'live' indefinitely until another script destroys it")] public float instanceLifeTime = 0;

    private void Start()
    {
        // if we haven't filled in a specific position, set instantiationTransform to this GameObjects transform
        if (instantiationTransform == null)
        {
            instantiationTransform = transform;
        }
    }
    
    //Instantiate the prefab on the given position and rotation when this method is called from a UnityEvent
    public void InstantiatePrefab()
    {
        GameObject instance = Instantiate(objectToInstantiate, instantiationTransform.position, instantiationTransform.rotation);
        
        // if instanceLifeTime is more than zero, destroy the instance after that much time
        if (instanceLifeTime > 0)
        {
            Destroy(instance, instanceLifeTime);
        }
    }
}
