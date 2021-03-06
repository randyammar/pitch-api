﻿using System;
using System.Collections.Generic;
using System.Linq;
using Pitch.Match.API.ApplicationCore.Engine.Events;

namespace Pitch.Match.API.ApplicationCore.Models.MatchResult
{
    public class MatchResult
    {
        public MatchResult(Match.Match match)
        {
            var matchEvents = match.Minutes.SelectMany(x => x.Events).ToList();

            var homeTeamEvents = matchEvents.Where(x => x.SquadId == match.HomeTeam.Squad.Id).ToList();
            var awayTeamEvents = matchEvents.Where(x => x.SquadId == match.AwayTeam.Squad.Id).ToList();

            HomeStats = GetStats(match, homeTeamEvents, match.HomeTeam.Squad.Id);
            AwayStats = GetStats(match, awayTeamEvents, match.AwayTeam.Squad.Id);

            HomeResult = new Result
            {
                Score = homeTeamEvents.Count(x => x is Goal),
                Scorers = GetScorers(match, homeTeamEvents, match.HomeTeam.Squad),
                Name = match.HomeTeam.Squad.Name
            };

            AwayResult = new Result
            {
                Score = awayTeamEvents.Count(x => x is Goal),
                Scorers = GetScorers(match, awayTeamEvents, match.AwayTeam.Squad),
                Name = match.AwayTeam.Squad.Name
            };

            var cards = match.HomeTeam.Squad.Lineup.SelectMany(x => x.Value).Concat(match.AwayTeam.Squad.Lineup.SelectMany(x => x.Value));
            cards = cards.Concat(match.HomeTeam.Squad.Subs ?? new Card[0]).Concat(match.AwayTeam.Squad.Subs ?? new Card[0]);

            Events = matchEvents.Where(x => x.ShowInTimeline).Select((matchEvent, i) => new Event()
            {
                Minute = i, //TODO extension method
                Name = matchEvent.Name,
                Card = cards.FirstOrDefault(c => c != null && c.Id == matchEvent.CardId),
                SquadName = match.HomeTeam.Squad.Id == matchEvent.SquadId
                        ? match.HomeTeam.Squad.Name
                        : match.AwayTeam.Squad.Name, //TODO sending repeated data
                CardId = matchEvent.CardId
            }).ToList();

            Minute = match.Elapsed;
            Expired = match.HasFinished;
            ExpiredOn = match.HasFinished ? match.KickOff.AddMinutes(90) : (DateTime?)null;
        }

        private static Stats GetStats(Match.Match match, IList<IEvent> homeTeamEvents, Guid teamId)
        {
            return new Stats()
            {
                Shots = homeTeamEvents.Count(x => (new Type[] { typeof(Goal), typeof(ShotOnTarget), typeof(ShotOffTarget) }).Contains(x.GetType())),
                ShotsOnTarget = homeTeamEvents.Count(x => (new Type[] { typeof(Goal), typeof(ShotOnTarget) }).Contains(x.GetType())),
                Possession = CalculatePossession(match, teamId),
                Fouls = homeTeamEvents.Count(x => (new Type[] { typeof(YellowCard), typeof(RedCard), typeof(Foul) }).Contains(x.GetType())),
                YellowCards = homeTeamEvents.Count(x => x is YellowCard),
                RedCards = homeTeamEvents.Count(x => x is RedCard)
            };
        }

        private static int CalculatePossession(Match.Match match, Guid teamId)
        {
            var stats = match.Minutes.Where(x => x.Stats != null).Select(x => x.Stats).ToList();
            if (!stats.Any()) return 0;
            return (int)Math.Round(stats.Count(x => x.SquadIdInPossession == teamId) / (double)stats.Count * 100);
        }

        private static IEnumerable<string> GetScorers(Match.Match match, IEnumerable<IEvent> events, Squad team)
        {
            var scorers = new List<string>();
            var goals = events.Where(x => x is Goal).Cast<Goal>();
            var playerCards = team.Lineup.SelectMany(x => x.Value).Concat(team.Subs ?? new Card[0]);
            foreach (var goal in goals)
            {
                var player = playerCards.FirstOrDefault(x => x.Id == goal.CardId);
                scorers.Add($"{player.Name} {0}'");
            }
            return scorers;
        }

        public int Minute { get; set; }

        public Result HomeResult { get; set; }
        public Result AwayResult { get; set; }

        public Stats HomeStats { get; set; }
        public Stats AwayStats { get; set; }

        //TODO Rename to timeline?
        public IList<Event> Events { get; set; }

        public bool Expired { get; set; }

        public DateTime? ExpiredOn { get; set; }
    }
}
