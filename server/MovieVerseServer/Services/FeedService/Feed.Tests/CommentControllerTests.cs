using Feed.Controllers;
using Feed.DTOs.Comment;
using Feed.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Feed.Tests
{
    public class CommentControllerTests
    {
        [Fact]
        public async Task CreateComment_ReturnsComment_WithData()
        {
            // Arrange
            CreateCommentDTO commentDTO = new()
            {
                UserId = "12345",
                PostId = 4,
                Text = "Creating comment test text",
            };
            var mockRepo = new Mock<ICommentRepository>();
            var controller = new CommentController(mockRepo.Object);

            // Act
            var result = await controller.CreateComment(commentDTO);

            // Assert
            Assert.IsType<ActionResult<CommentDTO>>(result);
        }
    }
}
