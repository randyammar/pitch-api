﻿using System;

namespace Pitch.Match.API.ApplicationCore.Engine.Events
{
    public class ShotOffTarget : EventBase, IEvent
    {
        public ShotOffTarget(int minute, Guid cardId, Guid squadId) : base(minute, cardId, squadId) { }

        public string Name => "Shot Off Target";

        public bool ShowInTimeline => true;
    }
}