
public class RevolverState
{
    public RevolverFSM _fsm = null;
    public Revolver _revolver = null;

    public string Name { get; private set; }

    public RevolverState(string name)
    {
        this.Name = name;
    }

    public virtual void Begin()
    {

    }

    public virtual void StateUpdate()
    {

    }

    public virtual void Exit()
    {

    }
}