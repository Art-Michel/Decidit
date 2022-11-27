
public class ArmState
{
    public ArmFSM _fsm = null;
    public Arm _arm = null;

    public string Name { get; private set; }

    public ArmState(string name)
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