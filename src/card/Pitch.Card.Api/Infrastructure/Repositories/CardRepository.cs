﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Pitch.Card.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pitch.Card.Api.Infrastructure.Repositories
{
    public interface ICardRepository
    {
        Task<IEnumerable<Models.Card>> GetAllAsync(CardRequestModel req, string userId);
        Task<IEnumerable<Models.Card>> GetAsync(IEnumerable<Guid> ids);
        Task<EntityEntry<Models.Card>> AddAsync(Models.Card card);
    }

    public class CardRepository : ICardRepository
    {
        private readonly CardDbContext _cardDbContext;

        public CardRepository(CardDbContext cardDbContext)
        {
            _cardDbContext = cardDbContext;
        }

        public async Task<IEnumerable<Models.Card>> GetAsync(IEnumerable<Guid> ids)
        {
            return await _cardDbContext.Cards.Where(x => ids.Contains(x.Id)).ToListAsync();
        }

        public async Task<EntityEntry<Models.Card>> AddAsync(Models.Card card)
        {
            var entry = await _cardDbContext.Cards.AddAsync(card);
            await _cardDbContext.SaveChangesAsync();
            return entry;
        }

        public async Task<IEnumerable<Models.Card>> GetAllAsync(CardRequestModel req, string userId)
        {
            var query = _cardDbContext.Cards.Where(x => x.UserId == userId);
            if (req.NotIn != null && req.NotIn.Any())
            {
                query = query.Where(x => !req.NotIn.Contains(x.Id));
            }
            return await query.OrderByDescending(x => x.Position == req.PositionPriority).ThenByDescending(x => x.Rating).Skip(req.Skip).Take(req.Take).ToListAsync();
        }
    }
}
