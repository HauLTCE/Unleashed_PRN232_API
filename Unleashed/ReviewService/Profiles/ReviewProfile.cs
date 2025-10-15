using AutoMapper;
using ReviewService.DTOs.Review;
using ReviewService.Models;

namespace ReviewService.Profiles
{
    public class ReviewProfile : Profile
    {
        public ReviewProfile()
        {
            CreateMap<Review, ReviewDto>().ReverseMap();
            CreateMap<CreateReviewDto, Review>().ReverseMap();
            CreateMap<UpdateReviewDto, Review>().ReverseMap();
        }
    }
}