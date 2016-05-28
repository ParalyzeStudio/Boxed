using UnityEngine;

[ExecuteInEditMode]
public class Bonus : MonoBehaviour
{
    private const float rotationSpeed = 120.0f; //360 degrees/sec

    public void Update()
    {
        float dt = Time.deltaTime;

        //Make the cube rotate along
        GameObjectAnimator cubeAnimator = this.GetComponent<GameObjectAnimator>();
        Vector3 rotationAxis = this.transform.rotation * Vector3.up;
        cubeAnimator.SetRotationAxis(rotationAxis);
        cubeAnimator.RotateBy(rotationSpeed * dt, dt);
    }
}
