using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static Define;

//�����ϰ� �־��� ���� �����ϴ� ����
public class RoomManager
{
    public static RoomManager Instance;

    private int _roomCnt = 0; //���� �Ϸ��� ���� ��
    private int _bossCnt = 0;
    private RoomListSO _roomList;    

    public void InitStage(RoomListSO listSo, int bossCnt)
    {
        _roomList = listSo;
        _bossCnt = bossCnt;
        _roomCnt = 0;
    }

    public Room LoadNextRoom(RoomType type)
    {
        Room roomPrefab = LoadRandomRoom(type);
        
        Room room = GameObject.Instantiate(roomPrefab, null);
        
        if(room.DoorList.Count == 1) //���� �Ѱ��ۿ� ������ ������ ���͹����� ����
        {
            room.DoorList[0].nextRoomType = IsABossRoom();
        }else if(room.DoorList.Count >= 2)
        {
            room.DoorList[0].nextRoomType = IsABossRoom();

            for(int i = 1; i < room.DoorList.Count; i++)
            {
                room.DoorList[i].nextRoomType = GetSpecialRoom();
            }
        }

        _roomCnt++;

        return room;
    }

    private RoomType IsABossRoom()
    {
        if (_roomCnt == _bossCnt - 1)
            return RoomType.Store;
        if (_roomCnt == _bossCnt)
            return RoomType.Boss;

        return RoomType.Monster;
    }

    private RoomType GetSpecialRoom()
    {
        float dice = Random.Range(0, 1f);
        //Ʈ���̳� ���̳�

        if (dice < 0.5f)
            return RoomType.Trap;
        else
            return RoomType.Heal;

    }


    public Room LoadRandomRoom(RoomType type)
    {
        FieldInfo field = typeof(RoomListSO).GetField($"{type.ToString().ToLower()}Rooms", BindingFlags.Instance | BindingFlags.Public);
        List<Room> list = field.GetValue(_roomList) as List<Room>;

        int randIdx = Random.Range(0, list.Count);

        return list[randIdx];
    }

}
