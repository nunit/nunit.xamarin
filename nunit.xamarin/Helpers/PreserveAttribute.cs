using System;

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
public sealed class PreserveAttribute : Attribute
{
    /// <summary>
    /// When used on a class rather than a property, ensures that all members of this type are preserved.
    /// </summary>
    public bool AllMembers;

    /// <summary>
    /// Flags the method as a method to preserve during linking if the container class is pulled in.
    /// </summary>
    public bool Conditional;

    /// <summary>
    /// Initializes a new instance of the <see cref="PreserveAttribute"/> class.
    /// </summary>
    /// <param name="allMembers">If set to <c>true</c> all members will be preserved.</param>
    /// <param name="conditional">If set to <c>true</c>, the method will only be preserved if the container class is preserved.</param>
    public PreserveAttribute(bool allMembers, bool conditional)
    {
        AllMembers = allMembers;
        Conditional = conditional;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PreserveAttribute"/> class.
    /// </summary>
    public PreserveAttribute()
    {
    }
}
