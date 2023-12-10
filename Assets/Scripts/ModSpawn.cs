using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Thor;
using UnityEngine;
using UnityEngine.Serialization;
using Grid = Thor.Grid;

// Token: 0x0200015A RID: 346
[TaskCategory("Mod")]
public class ModSpawn : Action
{
    // Token: 0x06000347 RID: 839 RVA: 0x0000E446 File Offset: 0x0000C646
    public override void OnReset()
    {
        m_scale = Vector3.one;
    }

    // Token: 0x06000348 RID: 840 RVA: 0x0000E458 File Offset: 0x0000C658
    public override TaskStatus OnUpdate()
    {
        if (m_targetData.targetType == Spawn.TargetType.Entity)
        {
            Entity value = m_targetData.targetEntity.Value;
            Vector3 b = m_offset.Value;
            if (m_offsetSpace == Space.Self)
            {
                b = value.transform.TransformDirection(m_offset.Value);
            }

            Vector3 position = value.transform.position;
            if (m_matchVisual.Value)
            {
                VisualExt extension = value.GetExtension<VisualExt>();
                if (extension != null)
                {
                    position = extension.Position;
                }
            }

            Vector3 position2 = position + b;
            if (m_walkable.Value)
            {
                position2 = Simulation.GetNearestWalkablePosition(position2, m_entity.Value.AgentTypeID);
            }

            Entity entity = SpawnAtPosition(position2, -1);
            AssignOutput(entity);
            return TaskStatus.Success;
        }

        if (m_targetData.targetType == Spawn.TargetType.ZonePosition)
        {
            Entity entity2 = SpawnAtPosition(m_targetData.targetPosition.Value + m_offset.Value, -1);
            AssignOutput(entity2);
            return TaskStatus.Success;
        }

        if (m_targetData.targetType == Spawn.TargetType.RoomPosition)
        {
            Vector3 vector = Game.Instance.Simulation.Zone.CurrentRoom.transform.position +
                             m_targetData.targetPosition.Value;
            if (m_walkable.Value)
            {
                vector = Simulation.GetNearestWalkablePosition(vector, m_entity.Value.AgentTypeID);
            }

            Entity entity3 = SpawnAtPosition(vector + m_offset.Value, -1);
            AssignOutput(entity3);
            return TaskStatus.Success;
        }

        if (m_targetData.targetType == Spawn.TargetType.RandomGridPosition)
        {
            Vector3 a;
            if (Game.Instance.Simulation.Zone.CurrentRoom.GetRandomGridPosition(
                    m_entity.Value.GetComponent<Grid>(), out a))
            {
                Entity entity4 = SpawnAtPosition(a + m_offset.Value, -1);
                AssignOutput(entity4);
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }

        if (m_targetData.targetType == Spawn.TargetType.RandomRoomPosition)
        {
            Vector3 randomPosition =
                Game.Instance.Simulation.Zone.CurrentRoom.GetRandomPosition(m_walkable.Value,
                    m_entity.Value.AgentTypeID);
            Entity entity5 = SpawnAtPosition(randomPosition + m_offset.Value, -1);
            AssignOutput(entity5);
            return TaskStatus.Success;
        }

        return TaskStatus.Failure;
    }

    // Token: 0x06000349 RID: 841 RVA: 0x0000E6C8 File Offset: 0x0000C8C8
    private Entity SpawnAtPosition(Vector3 position, int playerID)
    {
        if (m_flattenY.Value)
        {
            position = position.Flatten(Axis.Y);
        }

        Quaternion quaternion = Quaternion.Euler(m_rotation.Value);
        quaternion = ((m_matchRotation.Value != null)
            ? m_matchRotation.Value.transform.rotation
            : quaternion);
        if (m_entity.Value != null)
        {
            Entity entity;
            using (new FullAnimator.SpawnScope(m_playSpawnAnimation.Value ? FullAnimator.kSpawn : 0))
            {
                entity = Game.Instance.Simulation.SpawnEntity(m_entity.Value, position, quaternion, playerID,
                    m_owner.Value);
            }

            if (m_link.Value != null)
            {
                entity.Link = m_link.Value;
                m_link.Value.Link = entity;
            }

            entity.transform.localScale = m_scale.Value;
            if (m_asMinion.Value)
            {
                entity.AddTag("minion");
            }

            return entity;
        }

        return null;
    }

    // Token: 0x0600034A RID: 842 RVA: 0x0000E810 File Offset: 0x0000CA10
    private void AssignOutput(Entity entity)
    {
        if (m_output != null && !m_output.IsNone)
        {
            m_output.Value = entity;
        }

        if (m_outputList != null && !m_outputList.IsNone)
        {
            m_outputList.Add(entity);
        }
    }

    // Token: 0x040003BF RID: 959
    [SerializeField] private Spawn.TargetData m_targetData;

    // Token: 0x040003C0 RID: 960
    [SerializeField] private SharedEntity m_entity;

    // Token: 0x040003C1 RID: 961
    [SharedRequired] [SerializeField] private SharedEntity m_output;

    // Token: 0x040003C2 RID: 962
    [SharedRequired] [SerializeField] private SharedEntityList m_outputList;

    // Token: 0x040003C3 RID: 963
    [SerializeField] private SharedVector3 m_offset;

    // Token: 0x040003C4 RID: 964
    [SerializeField] private Space m_offsetSpace = Space.Self;

    // Token: 0x040003C5 RID: 965
    [SerializeField] private SharedVector3 m_rotation;

    // Token: 0x040003C6 RID: 966
    [SerializeField] private SharedVector3 m_scale = Vector3.one;

    // Token: 0x040003C7 RID: 967
    [SharedRequired] [SerializeField] private SharedEntity m_owner;

    // Token: 0x040003C8 RID: 968
    [SharedRequired] [SerializeField] private SharedEntity m_link;

    // Token: 0x040003C9 RID: 969
    [SharedRequired] [SerializeField] private SharedEntity m_matchRotation;

    // Token: 0x040003CA RID: 970
    [SerializeField] private SharedBool m_flattenY;

    // Token: 0x040003CB RID: 971
    [SerializeField] private SharedBool m_walkable;

    // Token: 0x040003CC RID: 972
    [SerializeField] private SharedBool m_matchVisual;

    // Token: 0x040003CD RID: 973
    [SerializeField] private SharedBool m_asMinion;

    // Token: 0x040003CE RID: 974
    [SerializeField] private SharedBool m_playSpawnAnimation = true;
}