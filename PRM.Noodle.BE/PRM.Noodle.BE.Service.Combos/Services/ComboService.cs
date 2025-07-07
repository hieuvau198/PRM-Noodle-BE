using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PRM.Noodle.BE.Service.Combos.Models;
using PRM.Noodle.BE.Share.Interfaces;
using PRM.Noodle.BE.Share.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM.Noodle.BE.Service.Combos.Services
{
    public class ComboService : IComboService
    {
        private readonly IGenericRepository<Combo> _comboRepo;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public ComboService(IGenericRepository<Combo> comboRepo, IUnitOfWork uow, IMapper mapper)
        {
            _comboRepo = comboRepo;
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ComboDto>> GetAllAsync()
        {
            var combos = await _comboRepo.GetAllAsync();
            return _mapper.Map<IEnumerable<ComboDto>>(combos);
        }

        public async Task<(IEnumerable<ComboDto> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string searchTerm = null, bool? isAvailable = null)
        {
            var query = _comboRepo.GetQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
                query = query.Where(c => c.ComboName.Contains(searchTerm) || (c.Description != null && c.Description.Contains(searchTerm)));

            if (isAvailable.HasValue)
                query = query.Where(c => c.IsAvailable == isAvailable);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(c => c.ComboId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (_mapper.Map<IEnumerable<ComboDto>>(items), totalCount);
        }

        public async Task<ComboDto> GetByIdAsync(int id)
        {
            var combo = await _comboRepo.GetByIdAsync(id);
            return combo == null ? null : _mapper.Map<ComboDto>(combo);
        }

        public async Task<ComboDto> CreateAsync(CreateComboDto dto)
        {
            var combo = _mapper.Map<Combo>(dto);
            combo.CreatedAt = DateTime.UtcNow;
            combo.UpdatedAt = DateTime.UtcNow;
            await _comboRepo.AddAsync(combo);
            await _uow.CompleteAsync();
            return _mapper.Map<ComboDto>(combo);
        }

        public async Task<bool> UpdateAsync(int id, UpdateComboDto dto)
        {
            var combo = await _comboRepo.GetByIdAsync(id);
            if (combo == null) return false;
            _mapper.Map(dto, combo);
            combo.UpdatedAt = DateTime.UtcNow;
            _comboRepo.Update(combo);
            await _uow.CompleteAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var combo = await _comboRepo.GetByIdAsync(id);
            if (combo == null) return false;
            _comboRepo.Remove(combo);
            await _uow.CompleteAsync();
            return true;
        }

        public async Task<IEnumerable<ComboDto>> GetAvailableAsync()
        {
            var combos = await _comboRepo.FindAsync(c => c.IsAvailable == true);
            return _mapper.Map<IEnumerable<ComboDto>>(combos);
        }

        public async Task<bool> PatchIsAvailableAsync(int comboId, bool isAvailable)
        {
            var combo = await _comboRepo.GetByIdAsync(comboId);
            if (combo == null) return false;
            combo.IsAvailable = isAvailable;
            combo.UpdatedAt = DateTime.UtcNow;
            _comboRepo.Update(combo);
            await _uow.CompleteAsync();
            return true;
        }
    }
}
