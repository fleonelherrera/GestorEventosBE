using API.Models;
using API.Models.Dto;
using AutoMapper;

namespace API
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Evento, EventoDto>().ReverseMap();
            CreateMap<Evento, EventoCreacionDto>().ReverseMap();
            CreateMap<Evento, EventoModificacionDto>().ReverseMap();
            CreateMap<Inscripcion, InscripcionResponseDto>().ReverseMap();
            CreateMap<Usuario, UsuarioDto>().ReverseMap();
            CreateMap<Usuario, UsuarioModificacionDto>().ReverseMap();
        }
    }
}
