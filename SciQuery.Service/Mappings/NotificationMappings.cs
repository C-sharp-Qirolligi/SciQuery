using AutoMapper;
using SciQuery.Domain.Entities;
using SciQuery.Service.DTOs.Notification;

namespace SciQuery.Service.Mappings;

public class NotificationMappings : Profile
{
    public NotificationMappings()
    {
        CreateMap<Notification, NotificationDto>().ReverseMap();    
    }
}
