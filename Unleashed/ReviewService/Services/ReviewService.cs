using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ReviewService.DTOs.Review;
using ReviewService.Models;
using ReviewService.Repositories.Interfaces;
using ReviewService.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReviewService.Services
{
    public class ReviewServicee : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;

        public ReviewServicee(IReviewRepository reviewRepository, IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ReviewDto>> GetAllReviewsAsync()
        {
            var reviews = await _reviewRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }

        public async Task<ReviewDto?> GetReviewByIdAsync(int id)
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            return _mapper.Map<ReviewDto>(review);
        }

        public async Task<ReviewDto> CreateReviewAsync(CreateReviewDto reviewDto)
        {
            var reviewEntity = _mapper.Map<Review>(reviewDto);
            var newReview = await _reviewRepository.AddAsync(reviewEntity);
            return _mapper.Map<ReviewDto>(newReview);
        }

        public async Task<bool> UpdateReviewAsync(int id, UpdateReviewDto reviewDto)
        {
            var reviewToUpdate = await _reviewRepository.GetByIdAsync(id);
            if (reviewToUpdate == null)
            {
                return false;
            }

            // Map only the fields from the DTO, preserving other values
            _mapper.Map(reviewDto, reviewToUpdate);

            try
            {
                await _reviewRepository.UpdateAsync(reviewToUpdate);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _reviewRepository.ExistsAsync(id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
            return true;
        }

        public async Task<bool> DeleteReviewAsync(int id)
        {
            if (!await _reviewRepository.ExistsAsync(id))
            {
                return false;
            }
            await _reviewRepository.DeleteAsync(id);
            return true;
        }
    }
}