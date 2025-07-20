using System;
using UnityEditor;
using UnityEngine;

namespace example
{
    [CustomEditor(typeof(RoomContainer))]
    public class RoomContainerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("FillContainer"))
            {
                FillContainer();
            }
        }

        private void FillContainer()
        {
            RoomContainer roomContainer = (RoomContainer)target;
            roomContainer.RoomGameObject = roomContainer.gameObject;

            roomContainer.DoorSprites = new SpriteRenderer[4];
            var sprites = roomContainer.GetComponentsInChildren<SpriteRenderer>();
            foreach (var spriteRenderer in sprites)
            {
                if (!spriteRenderer.name.Contains("DoorSprite")) continue;
                string spriteName = spriteRenderer.name.Replace("DoorSprite", "");
                roomContainer.DoorSprites[(int)Enum.Parse(typeof(ConnectionDirection), spriteName)]= spriteRenderer;
            }
            
            
        }
    }
}
