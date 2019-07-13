﻿using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pitch.Match.Api.Models
{
    public class Squad
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public IDictionary<string, IEnumerable<Card>> Lineup { get; set; }
        public Card[] Subs { get; set; }

        public void Substitute(Guid off, Guid on)
        {
            Card offCard = Lineup.Values.First(x => x.Any(y => y.Id == off)).First(); //TODO single doesnt work?
            Card onCard = Subs.Single(x => x.Id == on);

            foreach (var pos in Lineup.ToDictionary(x => x.Key, x => x.Value)) //TODO ?
            {
                var cards = pos.Value.ToList();
                var index = cards.FindIndex(x => x.Id == offCard.Id);
                if (index >= 0)
                {
                    cards[index] = onCard;
                    Lineup[pos.Key] = cards;
                }
            }

            var subs = Subs.ToList();
            var subIndex = subs.FindIndex(x => x.Id == onCard.Id);
            Subs[subIndex] = offCard;
        }
    }

    [BsonIgnoreExtraElements]
    public class Card
    {
        public Card()
        {

        }

        public Card(Card card)
        {
            Id = card.Id;
            Name = card.Name;
            ShortName = card.ShortName;
            Rating = card.Rating;
            Rarity = card.Rarity;
            Fitness = card.Fitness;
            Position = card.Position;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public int Rating { get; set; }
        public string Rarity { get; set; }
        public int Fitness { get; set; }
        public string Position { get; set; }
    }
}
