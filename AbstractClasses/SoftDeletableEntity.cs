namespace TicDrive.AbstractClasses
{
    public abstract class SoftDeletableEntity
    {
        public bool Removed { get; set; } = false;
    }
}
