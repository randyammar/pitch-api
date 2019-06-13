﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.AspNetCore.Http;
using Pitch.Store.Api.Application.Requests;
using Pitch.Store.Api.Application.Responses;
using Pitch.Store.Api.Infrastructure.Repositories;
using Pitch.Store.Api.Models;

namespace Pitch.Store.Api.Infrastructure.Services
{
    public interface IPackService
    {
        Task<IList<Pack>> GetAll(string userId);
        Task<CreateCardResponse> Open(Guid id, string userId);
        Task<Guid> Buy(Guid userId);
        Task CreateStartingPacksAsync(Guid userId);
    }
    public class PackService : IPackService
    {
        private readonly IPackRepository _packRepository;
        private readonly IBus _bus;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PackService(IPackRepository packRepository, IBus bus, IHttpContextAccessor httpContextAccessor)
        {
            _packRepository = packRepository;
            _bus = bus;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IList<Pack>> GetAll(string userId)
        {
            return await _packRepository.GetAllAsync(userId);
        }

        public async Task<CreateCardResponse> Open(Guid id, string userId)
        {
            var pack = await _packRepository.GetAsync(id);
            //TODO check logged in userid matches card userid
            var request = new CreateCardRequest(userId);
            return await _bus.RequestAsync<CreateCardRequest, CreateCardResponse>(request);
        }

        public async Task<Guid> Buy(Guid userId)
        {
            var pack = new Pack() { Id = Guid.NewGuid(), UserId = userId.ToString() };
            var @new = await _packRepository.AddAsync(pack);
            await _packRepository.SaveChangesAsync();
            return @new.Entity.Id; 
        }

        public async Task CreateStartingPacksAsync(Guid userId)
        {
            for (int i = 0; i < 16; i++) //TODO positional packs
            {
                var pack = new Pack() { Id = Guid.NewGuid(), UserId = userId.ToString() };
                await _packRepository.AddAsync(pack);
            }
            await _packRepository.SaveChangesAsync();
        }
    }
}
