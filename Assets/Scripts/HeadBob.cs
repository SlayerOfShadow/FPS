using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBob : MonoBehaviour
{
    [Range(0.001f, 0.01f)]
	public float Amount = 0.002f;
	
	[Range(1f, 30f)] 
	public float frequency = 10.0f;
	    
	[Range(10f, 100f)] 
	public float smooth = 10.0f;
	    
	Vector3 start_pos;
	
	void Start() {
    	
		start_pos = transform.localPosition;
    }

	void Update() {
    	
	    check_headbob_trigger();
		stop_head_bob();
    }
    
	private void check_headbob_trigger() {
        	
		float input_magnitude = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).magnitude;
	        
		if (input_magnitude > 0) {
		    	
			start_head_bob();
		}
	}
        
	private Vector3 start_head_bob() {
        	
		Vector3 pos = Vector3.zero;
		pos.y += Mathf.Lerp(pos.y, Mathf.Sin(Time.time * frequency) * Amount * 1.4f, smooth * Time.deltaTime);
		pos.x += Mathf.Lerp(pos.x, Mathf.Cos(Time.time * frequency / 2f) * Amount * 1.6f, smooth * Time.deltaTime);
		transform.localPosition += pos;
		    
		return pos;
	}
	
	private void stop_head_bob() {
        	
		if (transform.localPosition == start_pos) return;
		transform.localPosition = Vector3.Lerp(transform.localPosition, start_pos, 1 * Time.deltaTime);
		
	}
}
