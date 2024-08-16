namespace ArchetypeConfigurator;

public class Variable
{
    public bool Locked { get; private set; }
    public bool? Value { get; private set; }
    public int Id { get; }
    public bool IsUserDecision => !Locked && Value.HasValue && Value.Value;

    public void Lock()
    {
        if (Value is null) throw new InvalidOperationException("Cannot lock status without value");
        Locked = true;
    }

    public void UnlockAndReset()
    {
        Locked = false;
        Reset();
    }

    public void Reset()
    {
        if (Locked) throw new InvalidOperationException("Cannot set locked status");
        Value = null;
    }

    public void Set(bool value)
    {
        if (Locked) throw new InvalidOperationException("Cannot set locked status");
        Value = value;
    }

    public Variable(int id)
    {
        Id = id;
        Locked = false;
        Value = null;
    }

    protected bool Equals(Variable other)
    {
        return Locked == other.Locked && Value == other.Value && Id == other.Id;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Id == ((Variable)obj).Id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id);
    }
}