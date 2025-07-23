
using UnityEngine;


namespace SpriteMapper
{
    /// <summary> Contains modifiable app preferences, which are loaded at the start. </summary>
    public class Preferences
    {
        /// <summary> Dead zone radius in pixels for a held <see cref="Action"/> with dead zone conflict behaviour. </summary>
        public static float HoldDeadZoneRadius { get; private set; } = 15f;

        /// <summary> Timer length in seconds for a held <see cref="Action"/> with timer conflict behaviour. </summary>
        public static float HoldTimerLength { get; private set; } = 0.5f;
    }
}
