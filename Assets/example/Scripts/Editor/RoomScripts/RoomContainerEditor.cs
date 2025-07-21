using System;
using example.Scripts.RoomScripts;
using UnityEditor;
using UnityEngine;

namespace example.Scripts.Editor.RoomScripts
{
    [CustomEditor(typeof(RoomContainer))]
    public class RoomContainerEditor : UnityEditor.Editor
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
            var gameObjectProperty = serializedObject.FindProperty("RoomGameObject");
            gameObjectProperty.objectReferenceValue = roomContainer.gameObject;
            var transformObjectProperty = serializedObject.FindProperty("RoomTransform");
            transformObjectProperty.objectReferenceValue = roomContainer.transform;
            serializedObject.ApplyModifiedProperties();

            var spriteProperty = serializedObject.FindProperty("DoorSprites");
            spriteProperty.ClearArray();
            var sprites = roomContainer.GetComponentsInChildren<SpriteRenderer>();
            foreach (var spriteRenderer in sprites)
            {
                if (!spriteRenderer.name.Contains("DoorSprite")) continue;
                string spriteName = spriteRenderer.name.Replace("DoorSprite", "");
                InsertPropertyArrayElement(spriteProperty,(int)Enum.Parse(typeof(ConnectionDirection), spriteName),spriteRenderer);
            }

            Transform connectionPoints = null;
            Transform additionalItemPoints = null;
            Transform roomEffects = null;
            Transform roomBounds = null;

            foreach (Transform firstGenerationChild in roomContainer.RoomTransform)
            {
                if (firstGenerationChild.name.Contains("ConnectionPoints"))
                {
                    connectionPoints = firstGenerationChild;
                }
                else if (firstGenerationChild.name.Contains("AdditionalItemsPoints"))
                {
                    additionalItemPoints = firstGenerationChild;
                }
                else if (firstGenerationChild.name.Contains("RoomEffects"))
                {
                    roomEffects = firstGenerationChild;
                }
                else if (firstGenerationChild.name.Contains("RoomBounds"))
                {
                    roomBounds = firstGenerationChild;
                }
            }

            if (connectionPoints == null) DrawErrorMessage("Miss component in prefab: Connection points");
            else
            {
                var property = serializedObject.FindProperty("ConnectPoints");
                property.ClearArray();
                foreach (Transform connectionPoint in connectionPoints)
                {
                    int index;
                    if (connectionPoint.name.Contains("Left")) index = 0;
                    else if (connectionPoint.name.Contains("Right")) index = 1;
                    else if (connectionPoint.name.Contains("Up")) index = 2;
                    else if (connectionPoint.name.Contains("Down")) index = 3;
                    else
                    {
                        DrawErrorMessage("Undeclared connection point " + connectionPoint.name);
                        break;
                    }

                    InsertPropertyArrayElement(property, index, connectionPoint);
                }
            }

            if (additionalItemPoints == null) DrawErrorMessage("Miss component in prefab: Additional item points");
            else
            {
                var property = serializedObject.FindProperty("RoomAdditionalItemsPlaces");
                property.ClearArray();
                for (int i = 0; i < additionalItemPoints.childCount; i++)
                {
                    InsertPropertyArrayElement(property, i, additionalItemPoints.GetChild(i));
                }
            }

            if (roomEffects == null) DrawErrorMessage("Miss component in prefab: Room effects");
            else
            {
                var property = serializedObject.FindProperty("RoomEffects");
                property.ClearArray();
                roomContainer.RoomEffects = new ParticleSystem[roomEffects.childCount];
                for (int i = 0; i < roomEffects.childCount; i++)
                {
                    if (roomEffects.GetChild(i).TryGetComponent(out ParticleSystem particleSystem))
                    {
                        InsertPropertyArrayElement(property, i, particleSystem);
                    }
                }
            }

            if (roomBounds == null) DrawErrorMessage("Miss component in prefab: Room bounds");
            else
            {
                var property = serializedObject.FindProperty("RoomColliders");
                property.ClearArray();
                for (int i = 0; i < roomBounds.childCount; i++)
                {
                    if (!roomBounds.GetChild(i).TryGetComponent(out Collider2D roomBound)) continue;
                    InsertPropertyArrayElement(property, i, roomBound);
                }
            }
            
            serializedObject.ApplyModifiedProperties();
            
            PrefabUtility.RecordPrefabInstancePropertyModifications(roomContainer);
        }

        private void DrawErrorMessage(string missingItemName)
        {
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(missingItemName, MessageType.Error);
        }

        private void InsertPropertyArrayElement(SerializedProperty property, int index, UnityEngine.Object value)
        {
            property.InsertArrayElementAtIndex(index);
            property.GetArrayElementAtIndex(index).objectReferenceValue = value;
        }
    }
}
