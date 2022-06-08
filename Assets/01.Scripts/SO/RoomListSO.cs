using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Room/RoomList")]
public class RoomListSO : ScriptableObject
{
    public List<Room> startRooms;
    public List<Room> monsterRooms; //���Ͱ� �ִ� ��
    public List<Room> trapRooms; //Ʈ�� + ���͹� (��� �� ����)
    public List<Room> healRooms; //�ִ�ü������ + ���͹� 
    public List<Room> storeRooms; //������
    public List<Room> bossRooms; // ������
}
