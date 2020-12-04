using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;

namespace CoreCodeCamp.Profiles
{
    public class CampProfile: Profile
    {
        public CampProfile()
        {
           
            this.CreateMap<Camp, CampModel>()
                .ForMember(m => m.Venue, o => o.MapFrom(e => e.Location.VenueName))
                .ReverseMap();
            this.CreateMap<Location, LocationModel>().ReverseMap();
            this.CreateMap<Talk, TalkModel>()
                .ReverseMap()
                .ForMember(t => t.Camp, opt => opt.Ignore())
                .ForMember(t => t.Speaker, opt => opt.Ignore());
            this.CreateMap<Speaker, SpeakerModel>().ReverseMap();
        }
    }
}
