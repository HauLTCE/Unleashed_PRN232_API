using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReviewService.DTOs.Comment;
using ReviewService.Services.Interfaces;
using System.Security.Claims;

namespace ReviewService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        // POST: api/comments
        [HttpPost]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<ActionResult<CommentDto>> PostReply(CreateCommentDto commentDto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid replyingUserId;

            if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var parsedUserId))
            {
                replyingUserId = parsedUserId;
            }
            else
            {
                return Unauthorized("User ID could not be determined. Please login or provide a 'userId' in the query string for debugging.");
            }

            var createdComment = await _commentService.CreateReplyAsync(commentDto, replyingUserId);
            return CreatedAtAction(nameof(GetComment), new { id = createdComment.CommentId }, createdComment);
        }

        // DELETE: api/comments/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid currentUserId;

            if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var parsedUserId))
            {
                currentUserId = parsedUserId;
            }
            else
            {
                return Unauthorized("User ID could not be determined. Please login or provide a 'userId' in the query string for debugging.");
            }

            var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value);

            var deleteResult = await _commentService.DeleteCommentAsync(id, currentUserId, roles);
            if (!deleteResult) return NotFound();

            return NoContent();
        }

        // GET: api/comments/{id}/parent
        [HttpGet("{id}/parent")]
        public async Task<ActionResult<CommentDto>> GetCommentParent(int id)
        {
            var result = await _commentService.GetCommentParentAsync(id);
            return Ok(result);
        }

        // GET: api/comments/{id}/descendants
        [HttpGet("{id}/descendants")]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetCommentDescendants(int id)
        {
            var result = await _commentService.GetCommentDescendantsAsync(id);
            return Ok(result);
        }

        // GET: api/Comments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CommentDto>> GetComment(int id)
        {
            var comment = await _commentService.GetCommentByIdAsync(id);
            if (comment == null) return NotFound();
            return Ok(comment);
        }

        // PUT: api/Comments/5
        [HttpPut("{id}")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> PutComment(int id, UpdateCommentDto commentDto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid currentUserId;

            if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var parsedUserId))
            {
                currentUserId = parsedUserId;
            }
            else
            {
                return Unauthorized("User ID could not be determined. Please login or provide a 'userId' in the query string for debugging.");
            }

            var updateResult = await _commentService.UpdateCommentAsync(id, commentDto, currentUserId);
            if (!updateResult) return NotFound();
            return NoContent();
        }
    }
}