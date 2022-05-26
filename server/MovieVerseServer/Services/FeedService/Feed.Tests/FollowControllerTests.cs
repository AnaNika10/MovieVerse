using Feed.Controllers;
using Feed.DTOs.Follow;
using Feed.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
namespace Feed.Tests
{
    public class FollowControllerTests
    {
        private readonly Mock<IFollowRepository> repositoryStub = new();
        private readonly Random rand = new();
        [Fact]
        public async void GetUserFollowing_WithNonExistingUser_ReturnsNotFound()
        {

            //Arrange

            repositoryStub.Setup(repo => repo.GetUserFollowing(It.IsAny<String>())).ReturnsAsync((IEnumerable<FollowDTO>)null);
            var controller = new FollowController(repositoryStub.Object);

            //Act
            var result = await controller.GetUserFollowing(Guid.NewGuid().ToString());
            //Assert
            result.Result.Should().BeOfType<NotFoundResult>();

        }
        [Fact]
        public async void GetUserFollowing_WithExistingUser_ReturnsExpectedItem()
        {

            //Arrange
            string userId = "12345";
            var expected = CreateDummyItemsForFollowing(userId);
            repositoryStub.Setup(repo => repo.GetUserFollowing(userId))
                          .ReturnsAsync(expected);
            var controller = new FollowController(repositoryStub.Object);

            //Act
            var result = await controller.GetUserFollowing(userId);
            //Assert


            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var type = okResult.Value.Should().BeAssignableTo<IEnumerable<FollowDTO>>().Subject;
            var follow = okResult.Value.Should().BeEquivalentTo(expected);
        }
        [Fact]
        public async void GetUserFollowrs_WithNonExistingUser_ReturnsNotFound()
        {

            //Arrange

            repositoryStub.Setup(repo => repo.GetUserFollowers(It.IsAny<String>())).ReturnsAsync((IEnumerable<FollowDTO>)null);
            var controller = new FollowController(repositoryStub.Object);

            //Act
            var result = await controller.GetUserFollowers(Guid.NewGuid().ToString());
            //Assert
            result.Result.Should().BeOfType<NotFoundResult>();

        }
        [Fact]
        public async void GetUserFollowrs_WithExistingUser_ReturnsExpectedItem()
        {

            //Arrange
            string userId = "12345";
            var expected = CreateDummyItemsForFollowers(userId);
            repositoryStub.Setup(repo => repo.GetUserFollowers(userId))
                          .ReturnsAsync(expected);
            var controller = new FollowController(repositoryStub.Object);

            //Act
            var result = await controller.GetUserFollowers(userId);
            //Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var type = okResult.Value.Should().BeAssignableTo<IEnumerable<FollowDTO>>().Subject;
            var follow = okResult.Value.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async void CreateFollow_WithItemToCreate_ReturnsCreatedItem()
        {
            // Arrange
            CreateFollowDTO followToCreate = CreateRandomFollow();
            FollowDTO followToBeReturned = new FollowDTO();
            followToBeReturned.FollowToUserId = followToCreate.FollowToUserId;
            followToBeReturned.FollowFromUserId = followToCreate.FollowFromUserId;
            followToBeReturned.CreatedDate = followToCreate.CreatedDate;

            followToBeReturned.FollowId = 1;

            repositoryStub.Setup(repo => repo.GetById(It.IsAny<int>())).ReturnsAsync(followToBeReturned);
            var controller = new FollowController(repositoryStub.Object);

            // Act
            var result = await controller.CreateFollow(followToCreate);

            // Assert
            var CreatedAtRouteResult = result.Result.Should().BeOfType<CreatedAtRouteResult>().Subject;
            var createdfollow = CreatedAtRouteResult.Value.Should().BeAssignableTo<FollowDTO>().Subject;
            createdfollow.Should().NotBeNull();
            followToCreate.Should().BeEquivalentTo(
               createdfollow,
               options => options.ComparingByMembers<FollowDTO>().Excluding(x => x.FollowId)
           );

        }
        [Fact]
        public async void DeleteFollow_WithExistingItem_ReturnsOk()
        {
            // Arrange
            FollowDTO existingItem = GetRandomFollow();
            repositoryStub.Setup(repo => repo.DeleteFollow(existingItem.FollowId))
                    .ReturnsAsync(true);
            var controller = new FollowController(repositoryStub.Object);

            // Act
            var result = await controller.DeleteFollow(existingItem.FollowId);

            // Assert
            result.Result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async void DeleteItemAsync_WithNonExestingItem_ReturnsNotFound()
        {
            // Arrange
            FollowDTO existingItem = GetRandomFollow();
            repositoryStub.Setup(repo => repo.DeleteFollow(existingItem.FollowId))
                    .ReturnsAsync(false);
            var controller = new FollowController(repositoryStub.Object);

            // Act
            var result = await controller.DeleteFollow(existingItem.FollowId);

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }
        private IEnumerable<FollowDTO> CreateDummyItemsForFollowing(string userId)
        {
            var randNum1 = rand.Next(50);
            var randNum2 = rand.Next(50);
            var randNum3 = rand.Next(50);
            var r = new List<FollowDTO>();
            r.Add(new FollowDTO()
            {
                FollowId = randNum1,
                FollowFromUserId = userId,
                FollowToUserId = Guid.NewGuid().ToString(),
                CreatedDate = DateTime.Now
            });
            r.Add(new FollowDTO()
            {
                FollowId = randNum2,
                FollowFromUserId = userId,
                FollowToUserId = Guid.NewGuid().ToString(),
                CreatedDate = DateTime.Now
            });
            r.Add(new FollowDTO()
            {
                FollowId = randNum3,
                FollowFromUserId = userId,
                FollowToUserId = Guid.NewGuid().ToString(),
                CreatedDate = DateTime.Now
            });
            return r;



        }
        private CreateFollowDTO CreateRandomFollow()
        {
            //var randNum1 = rand.Next(50);
            return new()
            {
                FollowFromUserId = Guid.NewGuid().ToString(),
                FollowToUserId = Guid.NewGuid().ToString(),
                CreatedDate = DateTime.Now
            };
        }
        private FollowDTO GetRandomFollow()
        {
            var randNum1 = rand.Next(50);
            return new()
            {
                FollowId = randNum1,
                FollowFromUserId = Guid.NewGuid().ToString(),
                FollowToUserId = Guid.NewGuid().ToString(),
                CreatedDate = DateTime.Now
            };
        }
        private IEnumerable<FollowDTO> CreateDummyItemsForFollowers(string userId)
        {
            var randNum1 = rand.Next(50);
            var randNum2 = rand.Next(50);
            var randNum3 = rand.Next(50);
            var r = new List<FollowDTO>();
            r.Add(new FollowDTO()
            {
                FollowId = randNum1,
                FollowFromUserId = Guid.NewGuid().ToString(),
                FollowToUserId = userId,
                CreatedDate = DateTime.Now
            });
            r.Add(new FollowDTO()
            {
                FollowId = randNum2,
                FollowFromUserId = Guid.NewGuid().ToString(),
                FollowToUserId = userId,
                CreatedDate = DateTime.Now
            });
            r.Add(new FollowDTO()
            {
                FollowId = randNum3,
                FollowFromUserId = Guid.NewGuid().ToString(),
                FollowToUserId = userId,
                CreatedDate = DateTime.Now
            });
            return r;



        }
    }
}
