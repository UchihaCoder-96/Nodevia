namespace Nodevia.Execution;


public abstract class NodeBehavior
{
    public abstract NodeOutputs Evaluate(NodeInputs inputs);
}

