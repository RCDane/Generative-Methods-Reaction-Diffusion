using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class SDFTorus : SDFObject
{

    public float innerRadius;
    public float outerRadius;
    // approximate bound of torus from box
    public override (Vector3, Vector3) Bounds()
    {
        Vector3 min = new Vector3(-outerRadius, -(outerRadius-innerRadius)/2.0f, -outerRadius);
        Vector3 max = new Vector3(outerRadius, (outerRadius-innerRadius)/2.0f, outerRadius);

        Vector3[] boxCorners = new Vector3[] {
            new Vector3(min.x, min.y, min.z),
            new Vector3(min.x, min.y, max.z),
            new Vector3(min.x, max.y, min.z),
            new Vector3(min.x, max.y, max.z),
            new Vector3(max.x, min.y, min.z),
            new Vector3(max.x, min.y, max.z),
            new Vector3(max.x, max.y, min.z),
            new Vector3(max.x, max.y, max.z)
        };
        
        // scale box corners
        for (int i = 0; i < boxCorners.Length; i++)
        {
            boxCorners[i] = Vector3.Scale(boxCorners[i], transform.localScale);
        }

        // rotate box corners

        for (int i = 0; i < boxCorners.Length; i++)
        {
            boxCorners[i] = transform.rotation * boxCorners[i];
        }

        
        Vector3 boxMin = Vector3.one * float.MaxValue;
        Vector3 boxMax = Vector3.one * float.MinValue;
        for (int i = 0; i < boxCorners.Length; i++)
        {
            boxMin = Vector3.Min(boxMin, boxCorners[i]);
            boxMax = Vector3.Max(boxMax, boxCorners[i]);
        }
        boxMin += transform.position;
        boxMax += transform.position;
        return (boxMin, boxMax);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public override float Evaluate(Vector3 pos)
	{
		Quaternion LocalRotation = Quaternion.Inverse(transform.localRotation);

		Vector3 rotatedPosition = LocalRotation * (pos - transform.position);
        rotatedPosition += transform.position;
        float R = (outerRadius + innerRadius) * 0.5f;  // Major radius
        float r = (outerRadius - innerRadius) * 0.5f;  // Minor radius

		Vector3 diff = transform.position - rotatedPosition;

        Vector2 xz = new Vector2(diff.x, diff.z);
        Vector2 q = new Vector2(xz.magnitude - R, diff.y);
        return q.magnitude - r;

		
	}

    void DrawRing(Vector3 position, Vector3 offset, float scale){
        Vector3 oldPosition = Vector3.zero;
        for (int i = -1; i < 10; i++){
            float angle = i * Mathf.PI * 2 / 10;
            Vector3 newPosition = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
            newPosition = newPosition * scale;
            
            newPosition =  transform.localRotation * newPosition; 
            newPosition = newPosition + position + offset;
            if (i != -1)
                Gizmos.DrawLine(oldPosition, newPosition);
            oldPosition = newPosition;
        
        }
    }

    void OnDrawGizmos(){
        Gizmos.color = Color.red;
        DrawRing(transform.position, Vector3.zero, outerRadius);
        DrawRing(transform.position, Vector3.zero,  innerRadius);
        float diff = innerRadius + (outerRadius - innerRadius) /2;
        DrawRing(transform.position, new Vector3(0, (outerRadius - innerRadius) /2, 0),diff);
        DrawRing(transform.position, new Vector3(0, -(outerRadius - innerRadius) /2, 0), diff);

    }
}
