using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TestMediatR1.DbContext;
using TestMediatR1.Game;
using TestMediatR1.Item.Models;
using TestMediatR1.Lobby.Services;
using TestMediatR1.Player.Models;

namespace TestMediatR1.Hubs
{
    public class GameHub : Hub
    {
        private readonly GameService _gameService;
        private readonly MyDbContext _dbContext;

        public GameHub(GameService gameService, MyDbContext dbContext)
        {
            _gameService = gameService;
            _dbContext = dbContext;
        }

        public async Task AddToGroup(string gameId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId); 
        }

        public async Task GetItemsForPlayers(List<PlayerProps> players, string gameId)
        {
            var player1 = players[0];
            var player2 = players[1];

            var playerItems = new PlayerItemsModel();

            foreach(var itemId in player1.ItemIds)
            {
                var item = await _dbContext.tblItem.FirstOrDefaultAsync(i => i.Id == itemId);

                if(item != null)
                    playerItems.Player1Items.Add(item);
                else
                    Console.WriteLine("GameHub.GetItemsForPlayer2 item not found");
            }

            foreach(var itemId in player2.ItemIds)
            {
                var item = await _dbContext.tblItem.FirstOrDefaultAsync(i => i.Id == itemId);

                if (item != null)
                    playerItems.Player2Items.Add(item);
                else
                    Console.WriteLine("GameHub.GetItemsForPlayer2 item not found");
            }

            await Clients.Group(gameId).SendAsync("GameStartItems", playerItems);
        }

        public async Task UseConsumable(string gameId, PlayerItemsModel playerItems, int itemId, int playerId, string playerName)
        {
            var itemToRemove = new ItemModel();

            if(playerId == 1)
            {
                itemToRemove = playerItems.Player1Items.FirstOrDefault(i => i.Id == itemId);
                if(itemToRemove != null)
                    playerItems.Player1Items.Remove(itemToRemove);
            }
            else
            {
                itemToRemove = playerItems.Player2Items.FirstOrDefault(i => i.Id == itemId);
                if (itemToRemove != null)
                    playerItems.Player2Items.Remove(itemToRemove);
            }

            var newItems = playerItems;

            await Clients.Group(gameId).SendAsync("ItemUsed", newItems);
            await Clients.Group(gameId).SendAsync("ItemUsedMessage", playerName + " Used a " + itemToRemove!.Name + "!");
        }

        public async Task Shoot(GameModel game, string playerName, int type, string playerTurn, int consumable = 0)
        {
            var turn = playerTurn == game.Players![0].Name ? game.Players[1].Name : game.Players[0].Name;
            int damage = 1;
            var gameResult = new GameModel();
            gameResult.Bullets = game.Bullets;

            //Om det blir blank kula som skjuts
            if (type == 0)
            {
                damage = 0;
                gameResult.Bullets.Blanks -= 1;
            }

            //Om det blir riktig kula som skjuts och spelaren använde en såg
            if(type == 1)
            {
                if (consumable == 1)
                    damage = 2;

                gameResult.Bullets.Bullets -= 1;
            }

            //Dra av liv från spelare som blev skjuten, kan inte gå minus på liv
            PlayerProps player = game.Players.FirstOrDefault(p => p.Name == playerName);
            if(player != null)
            {
                player.Health -= damage;

                if (player.Health < 0)
                    player.Health = 0;
            }

            //Ställa om ifall någon spelare nått 0 liv
            if(game.Players.Any(p => p.Health == 0))
            {
                Random rdn = new Random();
                var health = rdn.Next(3, 6);

                foreach(var updatedPlayer in game.Players)
                {
                    updatedPlayer.Health = health;
                }
            }

            if(gameResult.Bullets.Bullets == 0 && gameResult.Bullets.Blanks == 0)
            {
                gameResult.Bullets = _gameService.GenerateBullets();
            }

            gameResult.GameId = game.GameId;
            gameResult.Players = game.Players;

            await Clients.Group(game.GameId!).SendAsync("PlayerTurn", turn);
            await Clients.Group(game.GameId!).SendAsync("ShootResult", gameResult);
        }
    }
}
