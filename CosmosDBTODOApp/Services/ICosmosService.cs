using CosmosDBTODOApp.Models;

namespace CosmosDBTODOApp.Services
{
    public interface ICosmosService
    {
        Task<IEnumerable<Item>> GetItemsAsync(string query);
        Task<Item> GetItemAsync(string id);
        Task AddItemAsync(Item item);
        Task UpdateItemAsync(string id, Item item);
        Task DeleteItemAsync(string id);
    }
}
