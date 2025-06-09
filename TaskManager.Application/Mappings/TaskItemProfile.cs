using AutoMapper;
using TaskManager.Domain.InputModels;
using TaskManager.Domain.ViewModels;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Mappings
{
    public class TaskItemProfile : Profile
    {
        public TaskItemProfile()
        {
            CreateMap<CreateTaskInputModel, TaskItem>()
                .ConstructUsing(src => new TaskItem(src.Priority))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Comments, opt => opt.Ignore())
                .ForMember(dest => dest.TaskHistories, opt => opt.Ignore())
                .ForMember(dest => dest.Project, opt => opt.Ignore())
                .ForMember(dest => dest.AssignedUser, opt => opt.Ignore());

            CreateMap<TaskItem, TaskViewModel>()
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project.Name))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.AssignedUser.Name))
                .ForMember(dest => dest.History, opt => opt.MapFrom(src => src.TaskHistories));

            CreateMap<TaskItem, UpdateTaskInputModel>().ReverseMap();

        }
    }
}
