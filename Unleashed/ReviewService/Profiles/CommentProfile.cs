using AutoMapper;
using ReviewService.DTOs.Comment;
using ReviewService.Models;

namespace ReviewService.Profiles
{
    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
            CreateMap<Comment, CommentDto>();
            CreateMap<CreateCommentDto, Comment>();
            CreateMap<UpdateCommentDto, Comment>();
        }
    }
}