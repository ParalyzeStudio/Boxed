using UnityEngine;

public class BrickAnimator : GameObjectAnimator
{
    public override void OnFinishRotating()
    {
        BrickRenderer brick = this.GetComponent<BrickRenderer>();
        brick.OnFinishRolling();
    }

    public override void OnFinishTranslating()
    {
        base.OnFinishTranslating();

        BrickRenderer brickRenderer = GetComponent<BrickRenderer>();
        if (brickRenderer.m_brickTeleporting) //hack possible because there is no other translation movement of the brick while its teleporting
        {
            brickRenderer.OnFinishTeleportation();
        }
    }
}
