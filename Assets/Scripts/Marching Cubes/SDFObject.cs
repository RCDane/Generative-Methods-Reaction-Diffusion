using UnityEngine;

public abstract class SDFObject : MonoBehaviour
{
    public Vector3 position {get {return transform.position;}}

    // Returns min and max bounds of object. Used for calculating bounds
    public abstract (Vector3, Vector3) Bounds();

    public abstract float Evaluate(Vector3 point);
}
