using AutoMapper;
using Settlement.Gaming.Criteria.Happy5;
using Settlement.InternalApi.HappyFiveEndpoints;

namespace Settlement.InternalApi;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<SearchMatchEndpointRequest, GetMatchesCriteria>();
    }
}