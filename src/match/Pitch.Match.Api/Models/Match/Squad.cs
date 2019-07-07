﻿using System;
using System.Collections.Generic;

namespace Pitch.Match.Api.Models
{
    public class Squad
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public IDictionary<string, IEnumerable<Card>> Lineup { get; set; }

        //TODO SUBS
    }

    public class Card
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public int Rating { get; set; }
        public string Rarity { get; set; }
        public int Fitness { get; set; }
        public string Position { get; set; }

        //TODO this wont work during reentrancy
        public bool SentOff { get; set; }
    }
}
