using System;

namespace AldursLab.WurmAssistant3.Core.Areas.CombatStats.Data.Combat
{
    public class CombatStats
    {
        public CombatStats(CombatActor actorOne, CombatActor actorTwo)
        {
            ActorOne = actorOne;
            ActorTwo = actorTwo;
        }

        public CombatActor ActorOne { get; private set; }
        public CombatActor ActorTwo { get; private set; }

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