using UnityEngine;
using RAIN.Action;
using RAIN.Representation;

[RAINAction]
public class Jump : RAINAction
{
    private Animator animator;
    public Expression JumpForce = new Expression();

    public override ActionResult Execute(RAIN.Core.AI ai)
    {
        animator = GameObject.Find("Boss/Animator").GetComponent<Animator>();

        if (!animator)
            return ActionResult.FAILURE;
        if (!JumpForce.IsValid)
            return ActionResult.FAILURE;

        float jumpForce = JumpForce.Evaluate<float>(ai.DeltaTime, ai.WorkingMemory);
        Transform t = GameObject.Find("Boss").transform;
        if (t.position.y <= 3)
        {
            t.position = Vector3.Lerp(t.position, t.position + Vector3.up*jumpForce, 1f);
        }

        return ActionResult.SUCCESS;
    }
}