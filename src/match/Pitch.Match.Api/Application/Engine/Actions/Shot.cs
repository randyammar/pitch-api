﻿using MongoDB.Bson.Serialization.Attributes;
using Pitch.Match.Api.Application.Engine.Events;
using Pitch.Match.Api.Models;
using System;
using System.Collections.Generic;


namespace Pitch.Match.Api.Application.Engine.Action
{
    public class Shot : IAction
    {
        [BsonIgnore]
        public decimal ChancePerMinute => 0.2m;

        [BsonIgnore]
        public IDictionary<PositionalArea, decimal> PositionalChance => new Dictionary<PositionalArea, decimal>()
        {
            { PositionalArea.GK, 0.0m },
            { PositionalArea.DEF, 0.10m },
            { PositionalArea.MID, 0.40m },
            { PositionalArea.ATT, 0.50m },
        };

        [BsonIgnore]
        public bool AffectsTeamInPossession => true;

        public IEvent SpawnEvent(Card card, Guid squadId, int minute, Models.Match match, out bool forceReRoll)
        {
            forceReRoll = false;

            //TODO OnTarget/OffTarget based on card/oppDEF & Goal based on card/oppGK 

            Random rnd = new Random();
            int randomNumber = rnd.Next(1, 4);

            if (randomNumber == 1)
                return new Goal(minute, card.Id, squadId);
            if (randomNumber == 2)
                return new ShotOnTarget(minute, card.Id, squadId);
            if (randomNumber == 3)
                return new ShotOffTarget(minute, card.Id, squadId);
            return null;
        }
    }
}
