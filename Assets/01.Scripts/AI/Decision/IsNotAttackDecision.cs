public class IsNotAttackDecision : AIDecision
{
    //�������̸� �������·� �ȳѾ��.
    public override bool MakeADecision()
    {
        
        return !_aIActionData.attack;
    }

}
