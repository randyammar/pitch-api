using System;
using System.Collections.Generic;
using Pitch.Match.Api.ApplicationCore.Engine;
using Pitch.Match.Api.ApplicationCore.Engine.Actions;
using Pitch.Match.Api.ApplicationCore.Engine.Providers;
using Pitch.Match.Api.ApplicationCore.Models;

namespace Pitch.Match.Api.Tests.Engine
{
    public abstract class MatchTestBase
    {
        protected Card _stubAwayPlayer;
        protected Card _stubAwaySub;

        protected Card _stubHomePlayer;

        protected Squad _stubHomeSquad;
        protected Card _stubHomeSub;
        protected ApplicationCore.Models.Match _stubMatch;
        protected MatchEngine _stubMatchEngine;

        protected MatchTestBase()
        {
            var randomnessProvider = new RandomnessProvider();
            var actions = new IAction[] {new Foul(randomnessProvider), new Shot()};
            _stubMatchEngine = new MatchEngine(actions);

            _stubHomePlayer = new Card
            {
                Id = Guid.NewGuid(),
                Name = "GK",
                Rating = 80,
                Fitness = 100
            };

            _stubHomeSub = new Card
            {
                Id = Guid.NewGuid(),
                Name = "ST",
                Rating = 40,
                Fitness = 100
            };

            _stubAwayPlayer = new Card
            {
                Id = Guid.NewGuid(),
                Name = "ST",
                Rating = 40,
                Fitness = 100
            };

            _stubAwaySub = new Card
            {
                Id = Guid.NewGuid(),
                Name = "ST",
                Rating = 40,
                Fitness = 100
            };

            var stubHomeTeamLineup = new Dictionary<string, IEnumerable<Card>>
            {
                {
                    "GK", new List<Card>
                    {
                        _stubHomePlayer
                    }
                },
                {
                    "DEF", new List<Card>
                    {
                        new Card
                        {
                            Id = Guid.NewGuid(),
                            Name = "LB",
                            Rating = 80,
                            Fitness = 100
                        },
                        new Card
                        {
                            Id = Guid.NewGuid(),
                            Name = "CB",
                            Rating = 80,
                            Fitness = 100
                        },
                        new Card
                        {
                            Id = Guid.NewGuid(),
                            Name = "CB",
                            Rating = 80,
                            Fitness = 100
                        },
                        new Card
                        {
                            Id = Guid.NewGuid(),
                            Name = "RB",
                            Rating = 80,
                            Fitness = 100
                        }
                    }
                },
                {
                    "MID", new List<Card>
                    {
                        new Card
                        {
                            Id = Guid.NewGuid(),
                            Name = "LM",
                            Rating = 80,
                            Fitness = 100
                        },
                        new Card
                        {
                            Id = Guid.NewGuid(),
                            Name = "CM",
                            Rating = 80,
                            Fitness = 100
                        },
                        new Card
                        {
                            Id = Guid.NewGuid(),
                            Name = "CM",
                            Rating = 80,
                            Fitness = 100
                        },
                        new Card
                        {
                            Id = Guid.NewGuid(),
                            Name = "RM",
                            Rating = 80,
                            Fitness = 100
                        }
                    }
                },
                {
                    "ATT", new List<Card>
                    {
                        new Card
                        {
                            Id = Guid.NewGuid(),
                            Name = "ST",
                            Rating = 80,
                            Fitness = 100
                        },
                        new Card
                        {
                            Id = Guid.NewGuid(),
                            Name = "ST",
                            Rating = 80,
                            Fitness = 100
                        }
                    }
                }
            };

            var stubAwayTeamLineup = new Dictionary<string, IEnumerable<Card>>
            {
                {
                    "GK", new List<Card>
                    {
                        new Card
                        {
                            Id = Guid.NewGuid(),
                            Name = "GK",
                            Rating = 40,
                            Fitness = 100
                        }
                    }
                },
                {
                    "DEF", new List<Card>
                    {
                        new Card
                        {
                            Id = Guid.NewGuid(),
                            Name = "LB",
                            Rating = 40,
                            Fitness = 100
                        },
                        new Card
                        {
                            Id = Guid.NewGuid(),
                            Name = "CB",
                            Rating = 40,
                            Fitness = 100
                        },
                        new Card
                        {
                            Id = Guid.NewGuid(),
                            Name = "CB",
                            Rating = 40,
                            Fitness = 100
                        },
                        new Card
                        {
                            Id = Guid.NewGuid(),
                            Name = "RB",
                            Rating = 40,
                            Fitness = 100
                        }
                    }
                },
                {
                    "MID", new List<Card>
                    {
                        new Card
                        {
                            Id = Guid.NewGuid(),
                            Name = "LM",
                            Rating = 40,
                            Fitness = 100
                        },
                        new Card
                        {
                            Id = Guid.NewGuid(),
                            Name = "CM",
                            Rating = 40,
                            Fitness = 100
                        },
                        new Card
                        {
                            Id = Guid.NewGuid(),
                            Name = "CM",
                            Rating = 40,
                            Fitness = 100
                        },
                        new Card
                        {
                            Id = Guid.NewGuid(),
                            Name = "RM",
                            Rating = 40,
                            Fitness = 100
                        }
                    }
                },
                {
                    "ATT", new List<Card>
                    {
                        _stubAwayPlayer,
                        new Card
                        {
                            Id = Guid.NewGuid(),
                            Name = "ST",
                            Rating = 40,
                            Fitness = 100
                        }
                    }
                }
            };

            _stubHomeSquad = new Squad
            {
                Id = Guid.NewGuid(),
                Lineup = stubHomeTeamLineup,
                Subs = new[]
                {
                    _stubHomeSub
                },
                Name = "Good FC"
            };

            var stubAwaySquad = new Squad
            {
                Id = Guid.NewGuid(),
                Lineup = stubAwayTeamLineup,
                Subs = new[]
                {
                    _stubAwaySub
                },
                Name = "Shitty FC"
            };

            var stubHomeUserId = Guid.NewGuid();
            var stubAwayUserId = Guid.NewGuid();

            _stubMatch = new ApplicationCore.Models.Match
            {
                Id = Guid.NewGuid(),
                KickOff = DateTime.Now,
                HomeTeam = new TeamDetails
                {
                    UserId = stubHomeUserId,
                    Squad = _stubHomeSquad
                },
                AwayTeam = new TeamDetails
                {
                    UserId = stubAwayUserId,
                    Squad = stubAwaySquad
                }
            };
        }

        protected void SimulateStubMatch()
        {
            _stubMatch = _stubMatchEngine.SimulateReentrant(_stubMatch);
        }
    }
}