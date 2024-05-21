using TestMediatR1.Lobby.Models;

namespace TestMediatR1.Lobby.Services
{
    public class LobbyService
    {
        private readonly List<LobbyModel> _lobbies;

        public LobbyService()
        {
            _lobbies = new List<LobbyModel>();
        }

        public IReadOnlyList<LobbyModel> GetLobbies()
        {
            return _lobbies!;
        }

        public async Task<IReadOnlyList<LobbyModel>> CreateLobby(string userName, string userId, string sessionId)
        {
            if (_lobbies.FirstOrDefault(i => i.CreatorId == userId) != null)
            {
                return null!;
            }

            var lobby = new LobbyModel 
            {
                GameId = sessionId,
                Creator = userName,
                CreatorId = userId
            };

            _lobbies.Add(lobby);
            return GetLobbies();
        }

        public void RemoveLobby(string gameId)
        {
            var target = _lobbies.Where(i => i.GameId == gameId).FirstOrDefault();
            if (target == null) 
                return;

            _lobbies.Remove(target);
        }
    }
}
