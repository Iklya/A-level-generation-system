using System.Collections.Generic;
using UnityEngine;

namespace example.Scripts.RoomScripts
{
    public class RoomContainer : MonoBehaviour
    {
        public Transform RoomTransform;
        public GameObject RoomGameObject;
        public RoomTypes RoomType;
        [Space]
        public Transform[] RoomAdditionalItemsPlaces;
        public Transform[] ConnectPoints;
        public SpriteRenderer[] DoorSprites;
        public ParticleSystem[] RoomEffects;
        public Collider2D[] RoomColliders;
        
        private List<ConnectionDirection> _roomConnectionDirections = new List<ConnectionDirection>();

        public void InitializeRoom()
        {
            foreach (var spriteRenderer in DoorSprites)
            {
                spriteRenderer.enabled = false;
            }
        }

        public void PlayRoomEffects()
        {
            foreach (var roomEffect in RoomEffects)
            {
                roomEffect.Play();
            }
        }

        public bool IsPointInRoomBound(Vector3 point)
        {
            bool isOverlapping = false;
            foreach (var roomCollider in RoomColliders)
            {
                isOverlapping |= roomCollider.OverlapPoint(point);
            }
            return isOverlapping;
        }
        
        public void AddConnectionDirection(int directionIndex)
        {
            _roomConnectionDirections.Add((ConnectionDirection)directionIndex);
            DoorEnable(directionIndex);
        }

        public ConnectionDirection GetLastConnectionDirection()
        {
            return _roomConnectionDirections[^1];
        }

        private void DoorEnable(int doorId)
        {
            DoorSprites[doorId].enabled = true;
        }
    }
}

