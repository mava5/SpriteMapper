using SpriteMapper;
using UnityEngine;

public class FinishToolQuickUse
{
    public FinishToolQuickUse()
    {
        App.Project.Action.ActiveLongActions[typeof(int)].longAction.Cancel();
    }
}
