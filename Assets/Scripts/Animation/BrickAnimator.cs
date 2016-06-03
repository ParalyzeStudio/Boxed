using UnityEngine;

public class BrickAnimator : GameObjectAnimator
{
    public override void OnFinishRotating()
    {
        BrickRenderer brick = this.GetComponent<BrickRenderer>();
        brick.OnFinishRolling();
    }
}
