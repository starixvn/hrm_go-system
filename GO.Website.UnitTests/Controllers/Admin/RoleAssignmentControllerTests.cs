namespace GO.Website.UnitTests.Controllers.Admin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.Services;
    using GO.UserControl.Client.Core.Models;
    using GO.UserControl.Client.Core.Models.User;
    using GO.UserControl.Client.Core.Services;
    using GO.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Mvc;
    using Models.RoleAssignment;
    using NSubstitute;
    using Website.Controllers.Admin;
    using Website.Criteria.RoleAssignment;
    using Xunit;

    public class RoleAssignmentControllerTests
    {
        private IUserControlSession userControlSession;
        private IBaseService baseService;
        private IUserService userService;
        private readonly Fixture fixture = new();

        [Fact]
        public void Index_WhenCalled_ReturnExpectedViewData()
        {
            // Arrange
            var controller = GetController();
            //Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("~/Views/Admin/RoleAssignment/Index.cshtml", viewResult.ViewName);
        }

        [Fact]
        public async Task GetUserRoles_WhenCalled_ReturnExpectedData()
        {
            // Arrange
            var controller = GetController();
            var criteria = fixture.Create<GetUserRoleCriteria>();
            var users = fixture.Create<IEnumerable<UserInfo>>();
            var roles = fixture.Create<IEnumerable<UserRole>>();

            baseService.FetchMultipleAsync<UserInfo, UserRole>(criteria)
                .Returns(new ValueTuple<IEnumerable<UserInfo>, IEnumerable<UserRole>>(users, roles));

            foreach (var user in users)
            {
                user.Roles = roles.Where(x => x.UserId == user.UserId).ToList();
            }
            //Act
            var result = await controller.GetUserRoles(criteria);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var totalItems = jsonResult.Value.GetType().GetProperty("totalItems")?.GetValue(jsonResult.Value);
            var data = jsonResult.Value.GetType().GetProperty("data")?.GetValue(jsonResult.Value);
            Assert.Equal(criteria.TotalRows, totalItems);
            Assert.Equal(users, data);
        }

        [Fact]
        public async Task AssignRole_WhenCalled_ReturnExpectedData()
        {
            // Arrange
            var controller = GetController();
            var param = fixture.Create<AssignRoleParam>();
            var command = fixture.Create<CommandResult>();

            userService.AssignRoleAsync(param).Returns(command);
            //Act
            var result = await controller.AssignRole(param);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(command, jsonResult.Value);
        }

        #region helper

        private RoleAssignmentController GetController()
        {
            userControlSession = Substitute.For<IUserControlSession>();
            baseService = Substitute.For<IBaseService>();
            userService = Substitute.For<IUserService>();

            return new RoleAssignmentController(
                userControlSession,
                baseService,
                userService
            );
        }

        #endregion helper
    }
}