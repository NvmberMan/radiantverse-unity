using UnityEngine;

namespace Main.Gameplay
{
    public interface IObstacleBehavior
    {
        void OnPlayerHit(GameObject player);
    }
}
