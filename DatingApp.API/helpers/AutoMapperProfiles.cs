using System.Linq;
using AutoMapper;
using DatingApp.API.Dtos;
using DatingApp.API.Models;

namespace DatingApp.API.helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListDto>()
                    .ForMember(destination => destination.PhotoUrl, memberOptions => {
                        memberOptions.MapFrom(
                            source => source.Photos.FirstOrDefault(photo => photo.IsMain).Url
                        );
                    })
                    .ForMember(destination => destination.Age, memberOptions => {
                        memberOptions.ResolveUsing(
                            source => source.birthDate.CalculateAge()
                        );
                    });
            CreateMap<User, UserForDetailDto>()
                    .ForMember(destination => destination.PhotoUrl, memberOptions => {
                        memberOptions.MapFrom(
                            source => source.Photos.FirstOrDefault(photo => photo.IsMain).Url
                        );
                    })
                    .ForMember(destination => destination.Age, memberOptions => {
                        memberOptions.ResolveUsing(
                            source => source.birthDate.CalculateAge()
                        );
                    });
            CreateMap<Photo, PhotoForDetailDto>();
        }
    }
}