using Feed.DTOs.Post;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using Feed.Repository.Interfaces;
using Feed.Controllers;
using Feed.Entities;
using System.Collections.Generic;
namespace Feed.Tests
{
    public class PostControllerTests
    {

        private readonly Mock<IPostRepository> repositoryStub = new();
        private readonly Random rand = new();
        [Fact]
        public async void GetPost_WithNonExistingUser_ReturnsNotFound()
        {

            //Arrange

            repositoryStub.Setup(repo => repo.GetPostByUser(It.IsAny<String>())).ReturnsAsync((IEnumerable<PostDTO>)null);
            var controller = new PostController(repositoryStub.Object);

            //Act
            var result = await controller.GetPostByUser(Guid.NewGuid().ToString());
            //Assert
            result.Result.Should().BeOfType<NotFoundResult>();

        }
        [Fact]
        public async void GetPost_WithExistingUser_ReturnsExpectedItem()
        {

            //Arrange
            string userId = "12345";
            var expected = CreateDummyItems(userId);
            repositoryStub.Setup(repo => repo.GetPostByUser(userId))
                          .ReturnsAsync(expected);
            var controller = new PostController(repositoryStub.Object);

            //Act
            var result = await controller.GetPostByUser(userId);
            //Assert


            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var type = okResult.Value.Should().BeAssignableTo<IEnumerable<PostDTO>>().Subject;
            var post = okResult.Value.Should().BeEquivalentTo(expected);
            //OkObjectResult response = Assert.IsType<OkObjectResult>(result.Result);
            //Assert.Equal(200, response.StatusCode);
            //Assert.Equal(expected, response.Value);

        }
        [Fact]
        public async void CreatePost_WithItemToCreate_ReturnsCreatedItem()
        {
            // Arrange
            CreatePostDTO postToCreate = CreateRandomPost();
            PostDTO postToBeReturned = new PostDTO();
            postToBeReturned.UserId = postToCreate.UserId;
            postToBeReturned.NumOfLikes = postToCreate.NumOfLikes;
            postToBeReturned.CreatedDate = postToCreate.CreatedDate;
            postToBeReturned.PostText = postToCreate.PostText;
            postToBeReturned.Hashtags = postToCreate.Hashtags;
            postToBeReturned.FilesUrls = postToCreate.FilesUrls;
            postToBeReturned.PostId = 1;
                 
            repositoryStub.Setup(repo => repo.GetById(It.IsAny<int>())).ReturnsAsync(postToBeReturned);     //repositoryStub.Setup(repo => repo.CreatePost(It.IsAny<CreatePostDTO>()));                  
            var controller = new PostController(repositoryStub.Object);

            // Act
            var result = await controller.CreatePost(postToCreate);

            // Assert
            var CreatedAtRouteResult = result.Result.Should().BeOfType<CreatedAtRouteResult>().Subject;
            var createdPost = CreatedAtRouteResult.Value.Should().BeAssignableTo<PostDTO>().Subject;
            createdPost.Should().NotBeNull();
            postToCreate.Should().BeEquivalentTo(
               createdPost,
               options => options.ComparingByMembers<PostDTO>().Excluding(x=> x.PostId)
           );
         
        }
        [Fact]
        public async void UpdatePost_ValidItem_ReturnsUpdatedItem()
        {
            // Arrange
            UpdatePostDTO postToUpdate = GetRandomPostToUpdate();
            PostDTO postToBeReturned = new PostDTO();
            postToBeReturned.UserId = postToUpdate.UserId;
            postToBeReturned.NumOfLikes = postToUpdate.NumOfLikes;
            postToBeReturned.CreatedDate = postToUpdate.CreatedDate;
            postToBeReturned.PostText = postToUpdate.PostText;
            postToBeReturned.Hashtags = postToUpdate.Hashtags;
            postToBeReturned.FilesUrls = postToUpdate.FilesUrls;
            postToBeReturned.PostId = postToUpdate.PostId;

            repositoryStub.Setup(repo => repo.GetById(It.IsAny<int>())).ReturnsAsync(postToBeReturned);
            var controller = new PostController(repositoryStub.Object);


            // Act
            var result = await controller.UpdatePost(postToUpdate);

            // Assert
            var CreatedAtRouteResult = result.Result.Should().BeOfType<CreatedAtRouteResult>().Subject;
            var updatedPost = CreatedAtRouteResult.Value.Should().BeAssignableTo<PostDTO>().Subject;
            postToBeReturned.Should().NotBeNull();
            postToBeReturned.Should().BeEquivalentTo(
               postToBeReturned,
               options => options.ComparingByMembers<PostDTO>().Excluding(x => x.PostId)
           );

        }

        [Fact]
        public async void DeleteItemAsync_WithExistingItem_ReturnsOk()
        {
            // Arrange
            PostDTO existingItem = GetRandomPost();
            repositoryStub.Setup(repo => repo.DeletePost(existingItem.PostId))
                    .ReturnsAsync(true);
            var controller = new PostController(repositoryStub.Object);

            // Act
            var result = await controller.DeletePost(existingItem.PostId);

            // Assert
            result.Result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async void DeleteItemAsync_WithNonExestingItem_ReturnsNotFound()
        {
            // Arrange
            PostDTO existingItem = GetRandomPost();
            repositoryStub.Setup(repo => repo.DeletePost(existingItem.PostId))
                    .ReturnsAsync(false);
            var controller = new PostController(repositoryStub.Object);

            // Act
            var result = await controller.DeletePost(existingItem.PostId);

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }



        private  IEnumerable<PostDTO> CreateDummyItems(string userId)
        {
            var randNum1 = rand.Next(50);
            var randNum2 = rand.Next(50);
            var randNum3 = rand.Next(50);
            var r = new List<PostDTO>();
            r.Add(new PostDTO()
            {
                PostId = randNum1,
                UserId = userId,
                PostText = "za testiranje" + randNum1,
                Hashtags = new string[] {"#viral","#test" },
                NumOfLikes = 0,
                FilesUrls = Array.Empty<string>(),
                CreatedDate = DateTime.Now
            });
            r.Add(new PostDTO()
            {
                PostId = randNum2,
                UserId = userId,
                PostText = "za testiranje" + randNum2,
                Hashtags = new string[] { "#viral", "#test" },
                NumOfLikes = 0,
                FilesUrls = Array.Empty<string>(),
                CreatedDate = DateTime.Now
            });
            r.Add(new PostDTO()
            {
                PostId = randNum3,
                UserId = userId,
                PostText = "za testiranje" + randNum3,
                Hashtags = new string[] { "#viral", "#test" },
                NumOfLikes = 0,
                FilesUrls = Array.Empty<string>(),
                CreatedDate = DateTime.Now
            });
            return r;



        }
        private PostDTO GetRandomPost()
        {
            var randNum1 = rand.Next(50);
            return new()
            {
                PostId = randNum1,
                UserId = "123",
                PostText = "za testiranje create",
                Hashtags = new string[] { "#viral", "#test" },
                NumOfLikes = 0,
                FilesUrls = Array.Empty<string>(),
                CreatedDate = DateTime.Now
            };
        }
        private UpdatePostDTO GetRandomPostToUpdate()
        {
            var randNum1 = rand.Next(50);
            return new()
            {
                PostId = randNum1,
                UserId = "123",
                PostText = "za testiranje create",
                Hashtags = new string[] { "#viral", "#test" },
                NumOfLikes = 0,
                FilesUrls = Array.Empty<string>(),
                CreatedDate = DateTime.Now
            };
        }

        private CreatePostDTO CreateRandomPost()
        {
            //var randNum1 = rand.Next(50);
            return new()
            {
                //PostId = randNum1,
                UserId = "123",
                PostText = "za testiranje create",
                Hashtags = new string[] { "#viral", "#test" },
                NumOfLikes = 0,
                FilesUrls = Array.Empty<string>(),
                CreatedDate = DateTime.Now
            };
        }

    }
}
