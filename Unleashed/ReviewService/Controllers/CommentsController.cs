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
        [Authorize] // Any authenticated user can reply
        public async Task<ActionResult<CommentDto>> PostReply(CreateCommentDto commentDto)
        {
            var replyingUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var createdComment = await _commentService.CreateReplyAsync(commentDto, replyingUserId);
            return CreatedAtAction(nameof(GetComment), new { id = createdComment.CommentId }, createdComment);
        }

        // DELETE: api/comments/5
        [HttpDelete("{id}")]
        [Authorize] // Service layer will check for ownership or admin role
        public async Task<IActionResult> DeleteComment(int id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value);

            var deleteResult = await _commentService.DeleteCommentAsync(id, userId, roles);
            if (!deleteResult) return NotFound();

            return NoContent();
        }

        // GET: api/comments/{id}/ancestors
        [HttpGet("{id}/ancestors")]
        public async Task<IActionResult> GetCommentAncestors(int id)
        {
            var result = await _commentService.GetCommentAncestorsAsync(id);
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
        [Authorize]
        public async Task<IActionResult> PutComment(int id, UpdateCommentDto commentDto)
        {
            var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var updateResult = await _commentService.UpdateCommentAsync(id, commentDto, currentUserId);
            if (!updateResult) return NotFound();
            return NoContent();
        }
    }
}