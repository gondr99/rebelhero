public class IsNotAttackDecision : AIDecision
{
    //공격중이면 추적상태로 안넘어간다.
    public override bool MakeADecision()
    {
        
        return !_aIActionData.attack;
    }

}
