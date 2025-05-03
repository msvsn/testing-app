using AutoMapper;
using StudentTestingApp.BLL.DTOs;
using StudentTestingApp.Core.Models;
using System.Linq;

namespace StudentTestingApp.BLL.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserViewDto>();
            CreateMap<UserRegisterDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role));

            CreateMap<Answer, AnswerDto>().ReverseMap();

            CreateMap<Question, QuestionDto>().ReverseMap();

            CreateMap<Test, TestViewDto>()
                .ForMember(dest => dest.QuestionCount, opt => opt.MapFrom(src => src.Questions.Count))
                .ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.Questions));

            CreateMap<TestCreateDto, Test>()
                 .ForMember(dest => dest.AuthorId, opt => opt.Ignore())
                 .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<TestResult, TestResultDto>()
                 .ForMember(dest => dest.StudentName, opt => opt.Ignore())
                 .ForMember(dest => dest.TestTitle, opt => opt.Ignore());
        }
    }
}
