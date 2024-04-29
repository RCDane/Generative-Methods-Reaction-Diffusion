using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float cameraMoveSpeed;

    public float cameraTurnSpeed;
    
    

    private void CameraTranslation(){
        bool wIn = Input.GetKey(KeyCode.W);
        bool aIn = Input.GetKey(KeyCode.A);
        bool sIn = Input.GetKey(KeyCode.S);
        bool dIn = Input.GetKey(KeyCode.D);

        float forwardSpeed = 0;
        
        if (wIn)
        {
            forwardSpeed = 1.0f;
        }
        else if (sIn)
        {
            forwardSpeed = -1.0f;
        }
        
        float rightSpeed = 0;
        
        if (dIn)
        {
            rightSpeed = 1.0f;
        }
        else if (aIn)
        {
            rightSpeed = -1.0f;
        }

        Vector3 forward = Camera.main.transform.forward;

        Vector3 cameraMove = transform.TransformDirection(Vector3.forward) * forwardSpeed + transform.TransformDirection(Vector3.right) * rightSpeed;
        
        cameraMove.Normalize();
        
        
        transform.Translate(cameraMove * (Time.deltaTime * cameraMoveSpeed), Space.World) ;
    }
    private void CameraRotation()
    {
        if (Input.GetKey(KeyCode.Space)){
            float mouseX = Input.GetAxis("Mouse X") * cameraTurnSpeed * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * cameraTurnSpeed * Time.deltaTime;

            transform.Rotate(Vector3.up, mouseX, Space.World);
            transform.Rotate(Vector3.right, -mouseY, Space.Self);
        }

    }
    // Update is called once per frame
    void Update()
    {
        CameraTranslation();
        CameraRotation();
    }
}
