using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace IdentityServer.Tests
{
    public static class HelperMethods
    {
        public static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
        {
            var userStore = new Mock<IUserStore<TUser>>();
            var userManager = new Mock<UserManager<TUser>>(userStore.Object, null, null, null, null, null, null, null, null);
            userManager.Object.UserValidators.Add(new UserValidator<TUser>());
            userManager.Object.PasswordValidators.Add(new PasswordValidator<TUser>());

            return userManager;
        }

        public static Mock<RoleManager<TRole>> MockRoleManager<TRole>() where TRole : class
        {
            var roleStore = new Mock<IRoleStore<TRole>>();
            var roleManager = new Mock<RoleManager<TRole>>(roleStore.Object, null, null, null, null);

            return roleManager;
        }

    }
}
