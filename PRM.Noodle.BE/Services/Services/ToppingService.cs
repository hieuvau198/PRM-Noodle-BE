using AutoMapper;
using Repositories.Interfaces;
using Repositories.Models;
using Services.DTOs.Topping;
using Services.Interfaces;

namespace Services.Services
{
    public class ToppingService : IToppingService
    {
        private readonly IGenericRepository<Topping> _toppingRepo;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public ToppingService(IGenericRepository<Topping> toppingRepo, IUnitOfWork uow, IMapper mapper)
        {
            _toppingRepo = toppingRepo;
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ToppingDto>> GetAllAsync()
        {
            var toppings = await _toppingRepo.GetAllAsync();
            return _mapper.Map<IEnumerable<ToppingDto>>(toppings);
        }

        public async Task<ToppingDto> GetByIdAsync(int id)
        {
            var topping = await _toppingRepo.GetByIdAsync(id);
            return topping == null ? null : _mapper.Map<ToppingDto>(topping);
        }

        public async Task<ToppingDto> CreateAsync(CreateToppingDto dto)
        {
            var topping = _mapper.Map<Topping>(dto);
            await _toppingRepo.AddAsync(topping);
            await _uow.CompleteAsync();
            return _mapper.Map<ToppingDto>(topping);
        }

        public async Task<ToppingDto> UpdateAsync(int id, UpdateToppingDto dto)
        {
            var topping = await _toppingRepo.GetByIdAsync(id);
            if (topping == null) return null;
            _mapper.Map(dto, topping);
            _toppingRepo.Update(topping);
            await _uow.CompleteAsync();
            return _mapper.Map<ToppingDto>(topping);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var topping = await _toppingRepo.GetByIdAsync(id);
            if (topping == null) return false;
            _toppingRepo.Remove(topping);
            await _uow.CompleteAsync();
            return true;
        }

        public async Task<IEnumerable<ToppingDto>> GetAvailableAsync()
        {
            var toppings = await _toppingRepo.FindAsync(t => t.IsAvailable == true);
            return _mapper.Map<IEnumerable<ToppingDto>>(toppings);
        }

        public async Task<bool> PatchIsAvailableAsync(int id, bool isAvailable)
        {
            var topping = await _toppingRepo.GetByIdAsync(id);
            if (topping == null) return false;
            topping.IsAvailable = isAvailable;
            _toppingRepo.Update(topping);
            await _uow.CompleteAsync();
            return true;
        }
    }
}