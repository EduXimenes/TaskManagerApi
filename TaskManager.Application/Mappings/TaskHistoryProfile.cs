using AutoMapper;
using TaskManager.Domain.ViewModels;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Mappings
{
    public class TaskHistoryProfile : Profile
    {
        public TaskHistoryProfile()
        {
            CreateMap<TaskHistory, TaskHistoryViewModel>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name));
        }
    }
}