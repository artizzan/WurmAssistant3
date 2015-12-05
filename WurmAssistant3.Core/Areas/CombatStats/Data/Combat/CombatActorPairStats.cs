using System;
using System.Linq;

namespace AldursLab.WurmAssistant3.Core.Areas.CombatStats.Data.Combat
{
    public class CombatActorPairStats
    {
        public CombatActorPairStats(CombatActor actorOne, CombatActor actorTwo)
        {
            ActorOne = actorOne;
            ActorTwo = actorTwo;

            PairId = string.Join(" vs ",
                new[] {actorOne, actorTwo}.OrderBy(actor => actor.Name).Select(actor => actor.Name));
        }

        public CombatActor ActorOne { get; private set; }
        public CombatActor ActorTwo { get; private set; }
        public string PairId { get; private set; }

        public CombatActor GetActorByName(string name)
        {
            if (ActorOne.Name == name)
            {
                return ActorOne;
            }
            else if (ActorTwo.Name == name)
            {
                return ActorTwo;
            }
            else
            {
                throw new InvalidOperationException("SourceActor and DestinationAction do not match name: " + (name ?? "NULL"));
            }
        }
    }
}