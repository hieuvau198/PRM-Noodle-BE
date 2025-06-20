using AutoMapper;
using Repositories.Interfaces;
using Repositories.Models;
using Services.DTOs.Combo;
using Services.DTOs.Product;
using Services.Interfaces;

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
