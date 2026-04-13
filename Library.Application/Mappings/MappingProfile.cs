using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Library.Application.DTOs;
using Library.Core.Entities;


namespace Library.Application.Mappings
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            // Book mappings
            CreateMap<Book, BookDto>();
            CreateMap<CreateBookDto, Book>();
            CreateMap<UpdateBookDto, Book>();

            // User mappings (for Auth responses)
            CreateMap<User, AuthResponseDto>()
                .ForMember(dest => dest.Token, opt => opt.Ignore()) // Token is set separately
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName));

        }
    }
}
