using UnityEngine;

public class CharacterRotation : MonoBehaviour
{
    public float rotationSpeed = 100f; // Speed of rotation in degrees per second

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // TODO: Change to use the new Input System when available
        //if (Input.GetMouseButton(0))
        //{
        //    float mouseX = Input.GetAxis("Mouse X");
        //    transform.Rotate(Vector3.up, -mouseX * rotationSpeed * Time.deltaTime, Space.World);
        //}
    }
}
