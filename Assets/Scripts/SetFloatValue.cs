using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Mod")]
public class SetFloatValue : Action
{ 
    
    [RequiredField]  [SerializeField] private SharedFloat source;
    
    [RequiredField] [SerializeField] private float value;

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
