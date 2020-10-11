using UnityEngine;
using RAIN.Action;
using RAIN.Core;
using RAIN.Representation;

[RAINDecision("CheckAnimParam")]
public class CheckAnimParam : RAINDecision {
    private Animator animator;
    public Expression ParameterName = new Expression();
    public Expression ParameterValue = new Expression();

    public override ActionResult Execute(AI ai)
    {
        animator = GameObject.Find("Animator").GetComponentInParent<Animator>();

        if (!animator)
            return ActionResult.FAILURE;

        if (!ParameterName.IsValid || !ParameterValue.IsValid)
            return ActionResult.FAILURE;

        string tParameter = ParameterName.Evaluate<string>(ai.DeltaTime, ai.WorkingMemory);
        bool tValue = ParameterValue.Evaluate<bool>(ai.DeltaTime, ai.WorkingMemory);
        if (animator.GetBool(tParameter.ToString()) == tValue){
            return ActionResult.SUCCESS;
        } else
        {
            return ActionResult.FAILURE;
        }
	}
}