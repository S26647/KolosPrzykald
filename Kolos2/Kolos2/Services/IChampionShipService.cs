using Kolos2.DTO;

namespace Kolos2.Services;

public interface IChampionShipService
{
    Task<TeamScoreResponseDTO> GetInfoForTeamInChampionships(int id);

    Task<int> AssingPlayerToTeamAsync(int idPlayer, int idTeam);
}