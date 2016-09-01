using UnityEngine;
using System.Collections;

public class MonoDemo : MonoBehaviour {

    void Awake()
    {
        Debug.Log("Awake");
    }

    void OnEnable()
    {
        Debug.Log("OnEnable");
    }

	// Use this for initialization
	void Start () {
        Debug.Log("Start");
    }
	
	// Update is called once per frame
	void Update () {
        Debug.Log("Update");
    }

    void FixedUpdate()
    {
        Debug.Log("FixedUpdate");
    }


    void LateUpdate()
    {
        Debug.Log("LateUpdate");
    }

    void OnDisable()
    {
        Debug.Log("OnDisable");
    }

    void OnDestroy()
    {
        Debug.Log("OnDestroy");
    }


    
}
