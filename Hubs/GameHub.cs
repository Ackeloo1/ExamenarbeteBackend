using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TestMediatR1.DbContext;
using TestMediatR1.Extensions;
using TestMediatR1.Game.Models;
using TestMediatR1.Game.Services;
using TestMediatR1.Item.Models;
using TestMediatR1.Lobby.Services;
using TestMediatR1.Player.Models;

namespace TestMediatR1.Hubs
{
    public class GameHub : Hub
    {
        private readonly GameService _gameService;
        private readonly MyDbContext _dbContext;
        private readonly LobbyService _lobbyService;

        public GameHub(GameService gameService, MyDbContext dbContext, LobbyService lobbyService)
        {
            _gameService = gameService;
            _dbContext = dbContext;
            _lobbyService = lobbyService;
        }

        public async Task AddToGroup(string gameId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId); 
        }

        public async Task GetItemsForPlayers(List<PlayerProps> players, string gameId, List<ItemModel> player1Items, List<ItemModel> player2Items)
        {
            var player1 = players[0];
            var player2 = players[1];

            var playerItems = new PlayerItemsModel();

            //Hämtar Items från databasen utifrån tidigare givna id's om item finns än.
            if (!player1Items.Any() && !player2Items.Any())
            {
                foreach (var itemId in player1.ItemIds!)
                {
                    var item = await _dbContext.tblItem.FirstOrDefaultAsync(i => i.Id == itemId);

                    if (item != null)
                        playerItems.Player1Items.Add(item);
                    else
                        Console.WriteLine("GameHub.GetItemsForPlayer2 item not found");
                }

                foreach (var itemId in player2.ItemIds!)
                {
                    var item = await _dbContext.tblItem.FirstOrDefaultAsync(i => i.Id == itemId);

                    if (item != null)
                        playerItems.Player2Items.Add(item);
                    else
                        Console.WriteLine("GameHub.GetItemsForPlayer2 item not found");
                }
            }
            //Fylla på items vid round-start om det fanns items sen tidigare
            else
            {
                Random rdn = new Random();

                var numberOfItems = rdn.Next(0, 5 - player1Items.Count);

                var randomItems = await _dbContext.tblItem
                    .OrderBy(r => Guid.NewGuid())
                    .Take(numberOfItems)
                    .ToListAsync();

                playerItems.Player1Items.AddRange(player1Items);
                playerItems.Player1Items.AddRange(randomItems);


                numberOfItems = rdn.Next(0, 5 - player2Items.Count);

                randomItems = await _dbContext.tblItem
                    .OrderBy(r => Guid.NewGuid())
                    .Take(numberOfItems)
                    .ToListAsync();

                playerItems.Player2Items.AddRange(player2Items);
                playerItems.Player2Items.AddRange(randomItems);
            }

            await Clients.Group(gameId).SendAsync("GameStartItems", playerItems);
        }

        public async Task UseConsumable(string gameId, PlayerItemsModel playerItems, int itemId, int playerId, string playerName)
        {
            var itemToRemove = new ItemModel();
            var newItems = playerItems;

            //kolla om spelare 1 eller 2 använt "item", sedan ta bort "item" ur listan
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

            //Visa nuvarande kula om "magnifying glass" använts
            bool revealBullet = false;
            if (itemToRemove != null && itemToRemove.Name!.ToLower() == "magnifyingglass")
                revealBullet = true;

            var messageData = new
            {
                NewItems = newItems,
                Message = playerName + " Used a " + itemToRemove!.Name + "!",
                RevealBullet = revealBullet
            };

            await Clients.Group(gameId).SendAsync("ItemUsed", messageData);
        }

        public async Task GenerateBullet(GameModel game)
        {
            Random rdn = new Random();

            int totalBullets = game.Bullets!.Bullets;
            int totalBlanks = game.Bullets!.Blanks;

            int bulletType;

            if (totalBullets > 0 && totalBlanks > 0)
            {
                //Beräkna sannorlikhet för vald kula baserat på befintliga blanks och bullets
                double bulletProbability = (double)totalBullets / (totalBullets + totalBlanks);
                bulletType = rdn.NextDouble() < bulletProbability ? 1 : 0;
            }
            else if (totalBullets > 0)
            {
                bulletType = 1;
            }
            else if (totalBlanks > 0)
            {
                bulletType = 0;
            }
            else
            {
                //säkerhetsåtgärd, ska inte kunna hända
                await Clients.Caller.SendAsync("Error", "No bullets or blanks left.");
                return;
            }

            await Clients.Group(game.GameId!).SendAsync("bulletType", bulletType);
        }

        public async Task Shoot(GameModel game, string playerName, string playerTurn, int bulletType, int consumable = 0)
        {
            if (bulletType != 0 && bulletType != 1)
            {
                await Clients.Caller.SendAsync("Error", "Invalid bullet type.");
                return;
            }

            var nextTurn = "";
            int damage = 1;
            var gameResult = new GameModel();
            gameResult.Bullets = game.Bullets;
            Random rdn = new Random();

            //Om det blir blank kula som skjuts
            if (bulletType == 0) 
            {
                if (gameResult.Bullets!.Blanks > 0)
                {
                    damage = 0;
                    gameResult.Bullets.Blanks -= 1;
                    nextTurn = game.Players!.First(p => p.Name == playerName).Name!;
                    //nextTurn = playerName == playerTurn ? playerTurn : game.Players!.First(p => p.Name == playerName).Name!;
                }
                else
                {
                    await Clients.Caller.SendAsync("Error", "No blanks available.");
                    return;
                }
            }
            //Om det blir riktig kula som skjuts och spelaren använde en såg
            else if (bulletType == 1)
            {
                if (gameResult.Bullets!.Bullets > 0)
                {
                    if (consumable == 1)
                        damage = 2;
                    gameResult.Bullets.Bullets -= 1;
                    nextTurn = game.Players!.First(p => p.Name != playerTurn).Name!;
                }
                else
                {
                    await Clients.Caller.SendAsync("Error", "No bullets available.");
                    return;
                }
            }

            //Dra av liv från spelare som blev skjuten, kan inte gå minus på liv
            PlayerProps shotPlayer = game.Players!.FirstOrDefault(p => p.Name == playerName)!;
            if(shotPlayer != null)
            {
                shotPlayer.Health -= damage;

                if (shotPlayer.Health < 0)
                    shotPlayer.Health = 0;
            }

            //Ställa om ifall någon spelare nått 0 liv
            if(game.Players!.Any(p => p.Health == 0))
            {
                ResultModel result = new ResultModel
                {
                    WinnerName = game.Players!.FirstOrDefault(p => p.Health != 0)!.Name!,
                    LoserName = game.Players!.FirstOrDefault(p => p.Health == 0)!.Name!
                };

                var health = rdn.Next(3, 6);

                foreach(var updatedPlayer in game.Players)
                {
                    updatedPlayer.Health = health;
                }

                await Clients.Group(game.GameId!).SendAsync("GameResult", result);
            }

            bool newRound = false;

            if (gameResult.Bullets!.Bullets == 0 && gameResult.Bullets.Blanks == 0)
            {
                gameResult.Bullets = _gameService.GenerateBullets();
                newRound = true;
            }

            gameResult.GameId = game.GameId;
            gameResult.Players = game.Players;

            var data = new
            {
                Turn = nextTurn,
                GameResult = gameResult,
                BulletType = bulletType,
                NewRound = newRound
            };

            await Clients.Group(game.GameId!).SendAsync("ShootResult", data);
        }

        public async Task LeaveGame(string gameId)
        {
            var userId = Context.GetHttpContext()!.GetClaim("userId");
            if (userId == null)
            {
                await Clients.Caller.SendAsync("Error", "Error leaving lobby");
                return;
            }

            //Hämta lobbyn
            var lobby = _lobbyService.GetLobbies().Where(l => l.GameId == gameId).FirstOrDefault();

            //Kolla lobbyn finns
            if (lobby == null)
                return;

            if (_lobbyService.GetLobbies().Any(i => i.CreatorId == userId))
                return;

            if (await _gameService.IsInGame(userId))
                return;

            //Ta bort lobbyn och medlemmen som lämnade
            _lobbyService.RemoveLobby(gameId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);

            var updatedLobbies = _lobbyService.GetLobbies();
            await Clients.All.SendAsync("UpdateLobbies", updatedLobbies);
        }
    }
}
