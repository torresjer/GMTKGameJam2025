using UnityEngine;

[CreateAssetMenu(fileName = "GameEvent", menuName = "Scriptable Objects/GameEvent")]
public class GameEvent : ScriptableObject
{
    // The GameEvent object is simply a container for the Action delegate.
    private System.Action listeners;
    public void Raise() { listeners?.Invoke(); }
    public void RegisterListener(System.Action listener) { listeners += listener; }
    public void UnregisterListener(System.Action listener) { listeners -= listener; }
}
