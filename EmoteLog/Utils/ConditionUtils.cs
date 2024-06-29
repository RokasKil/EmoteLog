using Dalamud.Game.ClientState.Conditions;

namespace EmoteLog.Utils
{
    public class ConditionUtils
    {
        public static readonly ConditionFlag[] CombatFlags = { ConditionFlag.InCombat };
        public static readonly ConditionFlag[] InstanceFlags = { ConditionFlag.BoundByDuty, ConditionFlag.BoundByDuty56, ConditionFlag.BoundByDuty95 };
        public static readonly ConditionFlag[] CutsceneFlags = { ConditionFlag.OccupiedInCutSceneEvent, ConditionFlag.WatchingCutscene, ConditionFlag.WatchingCutscene78 };
        public static bool AnyCondition(params ConditionFlag[] flags)
        {
            foreach (ConditionFlag flag in flags)
            {
                if (PluginServices.Condition[flag])
                {
                    return true;
                }

            }
            return false;
        }
    }
}
