using UnityEngine;

public class BrickAnimator : GameObjectAnimator
{
    public override void OnFinishRotating()
    {
        Brick brick = this.GetComponent<Brick>();
        brick.OnFinishRolling();
    }
}
