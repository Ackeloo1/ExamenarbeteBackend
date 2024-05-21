using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using TestMediatR1.Extensions;
using TestMediatR1.Game.Services;
using TestMediatR1.Lobby.Services;
using TestMediatR1.Player.Services;

namespace TestMediatR1.Hubs
{
    public class LobbyHub : Hub
    { 
        private readonly LobbyService _lobbyService;
        private readonly GameService _gameService;
        private readonly PlayerService _playerService;

        public LobbyHub(LobbyService lobbyService, GameService gameService, PlayerService playerService)
        {
            _lobbyService = lobbyService;
            _gameService = gameService;
            _playerService = playerService;
        }

        public async Task GetLobbies()
        {
            var lobbies = _lobbyService.GetLobbies();
            await Clients.All.SendAsync("UpdateLobbies", lobbies);
        }

        public async Task CreateLobby()
        {
            var userId = Context.GetHttpContext()!.GetClaim("userId");
            if (userId == null)
            {
                await Clients.Caller.SendAsync("Error", "CreateLobby error");
                return;
            }

            var player = (await _playerService.GetPlayers(new List<int>
            {
                int.Parse(userId)
            })).FirstOrDefault();

            if (player == null)
            {
                await Clients.Caller.SendAsync("Error", "Error som fan");
                return;
            }

            string sessionId = Guid.NewGuid().ToString();
            await _lobbyService.CreateLobby(player.Username!, player.Id.ToString(), sessionId);
            await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
            await GetLobbies();
        }

        public async Task JoinLobby(string gameId)
        {
            var userId = Context.GetHttpContext()!.GetClaim("userId");
            if (userId == null)
            {
                await Clients.Caller.SendAsync("Error", "Error joining lobby");
                return;
            }

            //Hämta lobbyn
            var lobby = _lobbyService.GetLobbies().Where(l => l.GameId == gameId).FirstOrDefault();

            //Kolla lobbyn finns
            if (lobby == null)
                return;

            //Hämta spelare 1 och 2 på id, CreatorId (spelare1) och userId (spelare2).
            var players = await _playerService.GetPlayers(new List<int>
            {
                int.Parse(userId),
                int.Parse(lobby.CreatorId!)
            });

            //Om getPlayers listan != 2 så har nåt gått snett, returnera wtf (bör inte hända).
            if(players.Count != 2)
                return;

            //Är spelare2 i ett game eller lobby redan? i så fall returnera error, annars woohoo
            if(_lobbyService.GetLobbies().Any(i => i.CreatorId == userId))
                return;
            if(await _gameService.IsInGame(userId))
                return;

            //Skapa game, returnera startGame objektet
            var gameStart = await _gameService.CreateGame(gameId, players);
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
            await Clients.Group(gameId).SendAsync("StartGame", gameStart);
            await GetLobbies();
        }

        //fungerar så fort frontend har tillgång.
        //public async Task LeaveLobby(string gameId)
        //{
        //    var userId = Context.GetHttpContext()!.GetClaim("userId");
        //    if (userId == null)
        //    {
        //        await Clients.Caller.SendAsync("Error", "Error leaving lobby");
        //        return;
        //    }

        //    //Hämta lobbyn
        //    var lobby = _lobbyService.GetLobbies().Where(l => l.GameId == gameId).FirstOrDefault();

        //    //Kolla lobbyn finns
        //    if (lobby == null)
        //        return;

        //    if (_lobbyService.GetLobbies().Any(i => i.CreatorId == userId))
        //        return;

        //    if (await _gameService.IsInGame(userId))
        //        return;
            
        //    //Ta bort lobbyn och medlemmen som lämnade
        //    _lobbyService.RemoveLobby(gameId);
        //    await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);

        //    var updatedLobbies = _lobbyService.GetLobbies();
        //    await Clients.All.SendAsync("UpdateLobbies", updatedLobbies);
        //}
    }
}
