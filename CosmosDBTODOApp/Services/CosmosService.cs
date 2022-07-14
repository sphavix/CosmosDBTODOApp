using CosmosDBTODOApp.Models;
using Microsoft.Azure.Cosmos;

namespace CosmosDBTODOApp.Services
{
    public class CosmosService : ICosmosService
    {
        private Container _container;
        public CosmosService(CosmosClient dbClient, string databaseName, string containerName)
        {
            this._container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task<IEnumerable<Item>> GetItemsAsync(string queryString)
        {
            //Get items from the container and iterate throu the list.
            var query = this._container.GetItemQueryIterator<Item>(new QueryDefinition(queryString));
            List<Item> items = new List<Item>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                items.AddRange(response.ToList());
            }

            return items;
        }

        public async Task<Item> GetItemAsync(string id)
        {
            try
            {
                ItemResponse<Item> response = await this._container.ReadItemAsync<Item>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task AddItemAsync(Item item)
        {
            await this._container.CreateItemAsync<Item>(item, new PartitionKey(item.Id));
        }

        public async Task UpdateItemAsync(string id, Item item)
        {
            await this._container.UpsertItemAsync<Item>(item, new PartitionKey(item.Id));
        }

        public async Task DeleteItemAsync(string id)
        {
            await this._container.DeleteItemAsync<Item>(id, new PartitionKey(id));
        }
    }
}
