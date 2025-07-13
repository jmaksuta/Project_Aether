using Project_Aether_Backend.Models;
using ProjectAether.Objects.Net._2._1.Standard.Models;

namespace Project_Aether_Backend.Shared
{
    public static class UserMapper
    {
        public static ApplicationUser? FromSharedUser(this User appUser)
        {
            if (appUser == null)
            {
                return null;
            }

            // Need to ensure Player navigation property is loaded if you want it
            // Example with eager loading if needed in your service:
            // var appUser = await _userManager.Users.Include(u => u.Player).FirstOrDefaultAsync(u => u.Id == userId);

            var sharedUser = new ApplicationUser
            {
                Id = appUser.Id,
                UserName = appUser.UserName,
                DateRegistered = appUser.DateRegistered
            };

            if (appUser.Player != null)
            {

                //sharedUser.Player = new PlayerProfile
                //{
                //    Id = appUser.Player.Id,
                //    PlayerName = appUser.Player.PlayerName,
                //    Characters = appUser.Player.Characters?.Select(c => PlayerCharacterMapper.FromSharedPlayerCharacter(c)).ToList(),
                //    ApplicationUser = sharedUser,
                //    ApplicationUserId = sharedUser.Id
                //    // Add mapping for Vector3 components if you are storing them as separate columns in the DB
                //    // For example:
                //    // Position = new ProjectAether.Shared.Models.CustomVector3(
                //    //    appUser.Player.PositionX, appUser.Player.PositionY, appUser.Player.PositionZ
                //    // )
                //};
            }

            return sharedUser;
        }

        public static User? ToSharedUser(this ApplicationUser appUser)
        {
            if (appUser == null)
            {
                return null;
            }

            // Need to ensure Player navigation property is loaded if you want it
            // Example with eager loading if needed in your service:
            // var appUser = await _userManager.Users.Include(u => u.Player).FirstOrDefaultAsync(u => u.Id == userId);

            var sharedUser = new ProjectAether.Objects.Net._2._1.Standard.Models.User
            {
                Id = appUser.Id,
                UserName = appUser.UserName,
                DateRegistered = appUser.DateRegistered
            };

            //if (appUser.Player != null)
            //{

            //    sharedUser.Player = new ProjectAether.Objects.Net._2._1.Standard.Models.PlayerProfile
            //    {
            //        Id = appUser.Player.Id, 
            //        PlayerName = appUser.Player.PlayerName,
            //        Characters = appUser.Player.Characters?.Select(c => c.ToSharedPlayerCharacter()).ToList(),
            //        User = sharedUser,
            //        UserId = sharedUser.Id
            //        // Add mapping for Vector3 components if you are storing them as separate columns in the DB
            //        // For example:
            //        // Position = new ProjectAether.Shared.Models.CustomVector3(
            //        //    appUser.Player.PositionX, appUser.Player.PositionY, appUser.Player.PositionZ
            //        // )
            //    };
            //}

            return sharedUser;
        }
    }
}
