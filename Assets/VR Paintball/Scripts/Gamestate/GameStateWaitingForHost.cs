public class GameStateWaitingForHost : GameState
{
    public override void EnterState()
    {
        // Do nothing while waiting for a host, network manager will shift us out of this state when there's a host.
    }

    public override void ExitState()
    {
        // Do nothing while waiting for a host, network manager will shift us out of this state when there's a host.
    }

    public override void Tick()
    {
        // Do nothing while waiting for a host, network manager will shift us out of this state when there's a host.
    }
}