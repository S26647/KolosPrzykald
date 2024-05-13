using Kolos2.Models;

namespace Kolos2.DTO;

public record TeamScoreResponseDTO(Championship Championship, IEnumerable<TeamWithScoreDTO> teamsDTOList);