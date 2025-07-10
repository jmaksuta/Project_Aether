using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Project_Aether_Backend_Test
{
    public static class MockUserManager
    {
        public static Mock<UserManager<TUser>> Create<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();

            var optionsAccessor = new Mock<IOptions<IdentityOptions>>();
            // Set up the Value property of IOptions<IdentityOptions>
            optionsAccessor.Setup(o => o.Value).Returns(new IdentityOptions());

            // Mock necessary dependencies. Only mock what you truly need to interact with in your tests.
            //var optionsAccessor = new Mock<IOptions<IdentityOptions>>();
            var passwordHasher = new Mock<IPasswordHasher<TUser>>();
            var userValidators = new List<IUserValidator<TUser>>(); // Or mock specific validators
            var passwordValidators = new List<IPasswordValidator<TUser>>(); // Or mock specific validators
            var lookupNormalizer = new Mock<ILookupNormalizer>();
            var errors = new Mock<IdentityErrorDescriber>();
            var serviceProvider = new Mock<IServiceProvider>();
            var logger = new Mock<ILogger<UserManager<TUser>>>();

            //var userManager = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
            var userManager = new Mock<UserManager<TUser>>(store.Object,
                optionsAccessor.Object,
                passwordHasher.Object,
                userValidators, // Pass the list
                passwordValidators, // Pass the list
                lookupNormalizer.Object,
                errors.Object,
                serviceProvider.Object,
                logger.Object
            );

            // Setup common methods that might be called (adjust as needed)
            userManager.Object.UserValidators.Add(new UserValidator<TUser>());
            userManager.Object.PasswordValidators.Add(new PasswordValidator<TUser>());

            // You might need to set up other services if your UserManager methods depend on them
            //userManager.Setup(u => u.Options).Returns(new Mock<IOptions<IdentityOptions>>().Object);
            // --- CORRECTED LINE HERE ---
            // We need to mock IOptions<IdentityOptions> and then set its Value property
            //var mockOptions = new Mock<IOptions<IdentityOptions>>();
            //mockOptions.Setup(o => o.Value).Returns(new IdentityOptions()); // Return a default IdentityOptions object
            //userManager.Setup(u => u.Options).Returns(new IdentityOptions());
            // --- END CORRECTED LINE ---
            // --- CORRECTED LINE HERE ---
            // We need to mock IOptions<IdentityOptions> and then set its Value property
            //var mockOptions = new Mock<IOptions<IdentityOptions>>();
            //mockOptions.Setup(o => o.Value).Returns(new IdentityOptions()); // Return a default IdentityOptions object
            //userManager.Setup(u => u.Options).Returns(mockOptions.Object); // <--- This is the corrected line
                                                                           // --- END CORRECTED LINE ---

            userManager.Setup(u => u.Logger).Returns(new Mock<ILogger<UserManager<TUser>>>().Object);
            //userManager.Setup(u => u.ErrorDescriber).Returns(new IdentityErrorDescriber());

            // Example: Setup FindByIdAsync if your controller uses it
            // This will be done per test case when you know the specific user data.
            // userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
            //            .ReturnsAsync((TUser)null); // Default to null, override in tests

            // Example: Setup GetRolesAsync
            // userManager.Setup(x => x.GetRolesAsync(It.IsAny<TUser>()))
            //            .ReturnsAsync(new List<string>());

            return userManager;
        }

        // Example: Setting up a basic option if needed
        //optionsAccessor.Setup(o => o.Value).Returns(new IdentityOptions());

        // Now you can set up methods on userManager or its dependencies
        //userManager.Setup(m => m.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((TUser)null); // Example setup

    }
}
