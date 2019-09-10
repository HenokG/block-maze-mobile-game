using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour {
    private void OnTriggerEnter(Collider other)
    {
    	if (other.CompareTag("CustomTrails"))
        	FindObjectOfType<PlayerController>().UndoMove();
    }
}
