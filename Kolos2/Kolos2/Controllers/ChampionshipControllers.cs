using Kolos2.DTO;
using Kolos2.Exceptions;
using Kolos2.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kolos2.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChampionshipControllers : ControllerBase
{
    private readonly IChampionShipService _championShipService;

    public ChampionshipControllers(IChampionShipService championShipService)
    {
        _championShipService = championShipService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetInfoAboutTeamOnChampionship(int id)
    {
        TeamScoreResponseDTO team = await _championShipService.GetInfoForTeamInChampionships(id);

        return Ok(team);
    }

    [HttpPost]
    public async Task<IActionResult> AssignPlayerToTeam(int idPlayer, int IdTeam)
    {
        int id;
        try
        {
            id = await _championShipService.AssingPlayerToTeamAsync(idPlayer, IdTeam);
        }
        catch (WrongAgeException)
        {
            return StatusCode(404, "Age is too small");
        }
        /*catch (PLayerDoesntExistException)
        {
            return StatusCode(404, "player doesnt Exist");
        }*/

        return Ok(id);
    }
}