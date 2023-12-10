using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Mod")]
public class SetIntValue : Action
{ 
    [RequiredField] [SerializeField] private SharedInt source;
    
    [RequiredField]  [SerializeField] private int value;

    [RequiredField] [SerializeField] private string op;

    public override TaskStatus OnUpdate()
    {
        switch (op)
        {
            case "=":
                source.Value = value;
                break;
            case "+=":
                source.Value += value;
                break;
            case "-=":
                source.Value -= value;
                break;
            case "*=":
                source.Value *= value;
                break;
            case "∕=":
                source.Value /= value;
                break;
        }
        return TaskStatus.Success;
    }
}
