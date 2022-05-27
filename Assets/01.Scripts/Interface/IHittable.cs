using UnityEngine;

public interface IHittable
{
    public bool IsEnemy { get; }
    Vector3 _hitPoint { get; }
    //�������̽����� ��������� �� �� ������ ���� ���͸� �����.
    //public UnityEvent<Vector3> OnGetHit { get; set; }
    public void GetHit(int damage, GameObject damageDealer);
}
