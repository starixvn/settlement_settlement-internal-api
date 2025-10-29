using AutoMapper;
using Fanex.Settlement.ScaleOut.Criteria.Update;
using Fanex.Settlement.ScaleOut.Request;
using Settlement.EventManagement.Criteria;
using Settlement.EventManagement.Endpoints;
using Settlement.Gaming.Criteria.Happy5;
using Settlement.Gaming.Endpoints;

namespace Settlement.InternalApi;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<RefreshMatchesEndpointRequest, GetWaitingMultiSportCriteria>();
        CreateMap<SearchMatchEndpointRequest, GetMatchesCriteria>();
        CreateMap<UpdateMatchInfo3RdRequest, UpdateMatchInfo3RdCriteria>();
    }
}