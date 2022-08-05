using System.Collections;
using GameModeLoader.Utils;
using ThunderRoad;

namespace Wully.MoreModes.Component
{
    public class Respawn : LevelModuleDeath
    {
        private int _lives = 3;
        public int lives = 3;
        

        public override IEnumerator OnLoadCoroutine()
        {
            _lives = lives;
            behaviour = Behaviour.Respawn;
            yield return base.OnLoadCoroutine();
            
        }
        protected override void OnPossessionEvent(Creature creature, EventTime eventTime)
        {
            if (eventTime == EventTime.OnEnd && Player.local?.creature)
            {
                Level.current.StartCoroutine(ShowLives());
            }
            base.OnPossessionEvent(creature, eventTime);
        }

        private IEnumerator ShowLives()
        {
            yield return Yielders.ForSeconds(4);
            Utilities.Message($"{_lives} lives left!");
        }
        
        protected override void OnCreatureKill(Creature creature, Player player, CollisionInstance collisionInstance, EventTime eventTime)
        {
            if (eventTime != EventTime.OnEnd || !player)
                return;

            if (_lives > 1)
            {
                _lives--;
            }
            else
            {
                behaviour = Behaviour.ShowScores;
            }
            base.OnCreatureKill(creature, player, collisionInstance, eventTime);
        }
        
        
    }
}
