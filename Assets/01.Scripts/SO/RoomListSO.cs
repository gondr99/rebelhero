using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Room/RoomList")]
public class RoomListSO : ScriptableObject
{
    public List<Room> monsterRooms; //���Ͱ� �ִ� ��
    public List<Room> trapRooms; //Ʈ�� + ���͹� (��� �� ����)
    public List<Room> healRooms; //�ִ�ü������ + ���͹� 
    public Room storeRoom; //������
    public Room bossRoom; // ������
}