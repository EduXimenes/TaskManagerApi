using AutoMapper;
using TaskManager.Application.InputModels;
using TaskManager.Application.ViewModels;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<CreateUserInputModel, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ForMember(dest => dest.Projects, opt => opt.Ignore())
                .ForMember(dest => dest.Tasks, opt => opt.Ignore())
                .ForMember(dest => dest.Comments, opt => opt.Ignore())
                .ForMember(dest => dest.TaskHistories, opt => opt.Ignore());

            CreateMap<User, UserViewModel>();
        }
    }
}
