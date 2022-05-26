namespace RPG.Control
{
    /// <summary>
    /// Script Component that Implement this must also Include any type Collider attached on same GameObject as that Script Component is attached to or on its parent.
    /// Otherwise the PlayerController.cs can't find the Script Component and implemented methods won't be called.
    /// </summary>
    public interface IRaycastable
    {
        public CursorType GetCursorType();
        public bool HandleRaycast(PlayerController playerController);
    }
}