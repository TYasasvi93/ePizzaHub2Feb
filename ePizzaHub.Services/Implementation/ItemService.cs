using ePizzaHub.Core.Entities;
using ePizzaHub.Models;
using ePizzaHub.Repositories.Interfaces;
using ePizzaHub.Services.Interfaces;

namespace ePizzaHub.Services.Implementation
{
    public class ItemService : Service<Item>, IItemService
    {
        IRepository<Item> _itemRepo;
        public ItemService(IRepository<Item> itemRepo) : base(itemRepo)
        {
            _itemRepo = itemRepo;
        }
        IEnumerable<ItemModel> IItemService.GetAll()
        {
            var data = _itemRepo.GetAll().OrderBy(i => i.CategoryId).ThenBy(i => i.ItemTypeId).Select(i => new ItemModel
            {
                Id = i.Id,
                Name = i.Name,
                Description = i.Description,
                ImageUrl = i.ImageUrl,
                ItemTypeId = i.ItemTypeId,
                UnitPrice = i.UnitPrice,
                CategoryId = i.CategoryId
            });
            return data;
        }
    }
}
