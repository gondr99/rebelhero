using UnityEngine;

public interface IHittable
{
    public bool IsEnemy { get; }
    Vector3 _hitPoint { get; }
    //인터페이스에는 멤버변수가 들어갈 수 없으니 겟터 셋터만 만든다.
    //public UnityEvent<Vector3> OnGetHit { get; set; }
    public void GetHit(int damage, GameObject damageDealer);
}
