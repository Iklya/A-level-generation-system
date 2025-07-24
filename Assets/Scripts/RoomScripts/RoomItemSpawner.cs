using UnityEngine;
using System.Collections.Generic;

public class RoomItemSpawner : MonoBehaviour
{
    public List<ItemTypeData> itemTypeDatas;
    public List<RoomItemChance> roomItemChances;

    public void SpawnItemsInAllRooms(RoomPlacer roomPlacer)
    {
        foreach (var room in roomPlacer.mainPath)
            SpawnItemsInRoom(room);

        foreach (var extraList in roomPlacer.extraPaths.Values)
        {
            foreach (var extraPath in extraList)
            {
                foreach (var room in extraPath.Path)
                    SpawnItemsInRoom(room);
            }
        }
    }

    public void SpawnItemsInRoom(RoomData roomData)
    {
        if (roomData.ItemPlaces == null || roomData.ItemPlaces.Count == 0)
            return;

        HashSet<ItemType> spawnedTypes = new HashSet<ItemType>();

        foreach (Transform place in roomData.ItemPlaces)
        {
            foreach (var chance in roomItemChances)
            {
                if (chance.AllowedRoomType != roomData.RoomType)
                    continue;

                if (spawnedTypes.Contains(chance.ItemType))
                    continue;

                float roll = Random.Range(0f, 100f);
                Debug.Log($"Комната {roomData.RoomType} появилась с шансом {chance.SpawnChance}%");

                if (roll <= chance.SpawnChance)
                {
                    GameObject prefab = GetRandomPrefabFromType(chance.ItemType);
                    if (prefab != null)
                    {
                        Instantiate(prefab, place.position, Quaternion.identity, place);
                        spawnedTypes.Add(chance.ItemType);

                        break;
                    }
                }
            }
        }
    }

    private GameObject GetRandomPrefabFromType(ItemType type)
    {
        ItemTypeData data = itemTypeDatas.Find(d => d.Type == type);

        return data.Prefabs[Random.Range(0, data.Prefabs.Count)];
    }
}
