using AgeOfHeroes.MapEditor;
using UnityEngine;

namespace AgeOfHeroes
{
    public abstract class AbstractMarker : MonoBehaviour, ILoadFromSerializable
    {
        public abstract void LoadFromSerializable(SerializableEntity serializableEntity);
    }
}