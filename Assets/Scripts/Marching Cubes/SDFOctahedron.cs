using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class SDFOctahedron : SDFObject
{

    public float height;
    // approximate bound of torus from box
    public override (Vector3, Vector3) Bounds()
    {
        Vector3 min = new Vector3(-height, -height, -height);
        Vector3 max = new Vector3(height, height, height);

        min = Vector3.Scale(min, transform.localScale);
        max = Vector3.Scale(max, transform.localScale);
        min += transform.position;
        max += transform.position;
        return (min, max);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Quaternion InverseRotation;
    private bool InverseRotationCalculated = false;
    public override float Evaluate(Vector3 pos)
	{
        if (!InverseRotationCalculated){
            InverseRotation = Quaternion.Inverse(transform.localRotation);
            InverseRotationCalculated = true;
        }

		Vector3 rotatedPosition = InverseRotation * (pos - transform.position);
        rotatedPosition += transform.position;

		Vector3 diff = transform.position - rotatedPosition;
        
        // Vector3 diff = transform.position - pos;
        Vector3 p = new Vector3(Mathf.Abs(diff.x), Mathf.Abs(diff.y), Mathf.Abs(diff.z));

        return (p.x + p.y + p.z - height) * 0.57735027f;
	}

    void OnDrawGizmos(){
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + new Vector3(-height, 0, -height), transform.position + new Vector3(height, 0, -height));
        Gizmos.DrawLine(transform.position + new Vector3(-height, 0, -height), transform.position + new Vector3(-height, 0, height));
        Gizmos.DrawLine(transform.position + new Vector3(height, 0, height), transform.position + new Vector3(height, 0, -height));
        Gizmos.DrawLine(transform.position + new Vector3(height, 0, height), transform.position + new Vector3(-height, 0, height));

        Gizmos.DrawLine(transform.position + new Vector3(-height, 0, -height), transform.position + new Vector3(0, height, 0));
        Gizmos.DrawLine(transform.position + new Vector3(-height, 0, height), transform.position + new Vector3(0, height, 0));
        Gizmos.DrawLine(transform.position + new Vector3(height, 0, -height), transform.position + new Vector3(0, height, 0));
        Gizmos.DrawLine(transform.position + new Vector3(height, 0, height), transform.position + new Vector3(0, height, 0));  

        Gizmos.DrawLine(transform.position + new Vector3(-height, 0, -height), transform.position + new Vector3(0, -height, 0));
        Gizmos.DrawLine(transform.position + new Vector3(-height, 0, height), transform.position + new Vector3(0, -height, 0));
        Gizmos.DrawLine(transform.position + new Vector3(height, 0, -height), transform.position + new Vector3(0, -height, 0));
        Gizmos.DrawLine(transform.position + new Vector3(height, 0, height), transform.position + new Vector3(0, -height, 0));        
    }
}
