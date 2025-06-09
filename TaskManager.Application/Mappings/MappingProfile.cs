using AutoMapper;
using TaskManager.Domain.Entities;
using TaskManager.Domain.InputModels;
using TaskManager.Domain.ViewModels;

namespace TaskManager.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Task mappings
            CreateMap<TaskItem, TaskViewModel>()
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project.Name))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.AssignedUser.Name))
                .ForMember(dest => dest.History, opt => opt.MapFrom(src => src.TaskHistories));

            CreateMap<CreateTaskInputModel, TaskItem>();
            CreateMap<UpdateTaskInputModel, TaskItem>();

            // TaskHistory mappings
            CreateMap<TaskHistory, TaskHistoryViewModel>()
                .ForMember(dest => dest.ModifiedByUserName, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.TaskStatusEnum, opt => opt.MapFrom(src => src.TaskStatusEnum));

            // Project mappings
            CreateMap<Project, ProjectViewModel>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name));

            CreateMap<CreateProjectInputModel, Project>();

            // User mappings
            CreateMap<User, UserViewModel>();

            CreateMap<CreateUserInputModel, User>();

            // Comment mappings
            CreateMap<Comment, CommentViewModel>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name));

            CreateMap<CreateCommentInputModel, Comment>();
        }
    }
} 