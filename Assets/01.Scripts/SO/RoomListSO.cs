using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Room/RoomList")]
public class RoomListSO : ScriptableObject
{
    public List<Room> startRooms;
    public List<Room> monsterRooms; //몬스터가 있는 방
    public List<Room> trapRooms; //트랩 + 몬스터방 (골드 더 많이)
    public List<Room> healRooms; //최대체력증가 + 몬스터방 
    public List<Room> storeRooms; //상점룸
    public List<Room> bossRooms; // 보스방
}
