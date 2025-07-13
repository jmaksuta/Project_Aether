
namespace Project_Aether_Backend.Shared
{
    public static class PlayerProfileMapper
    {
        public static Models.PlayerProfile? FromSharedPlayerProfile(ProjectAether.Objects.Net._2._1.Standard.Models.PlayerProfile playerProfile)
        {
            if (playerProfile == null)
            {
                return null;
            }
            return new Models.PlayerProfile
            {
                Id = playerProfile.Id,
                PlayerName = playerProfile.PlayerName,
                Characters = playerProfile.Characters?.Select(c => PlayerCharacterMapper.FromSharedPlayerCharacter(c)).ToList(),
                ApplicationUserId = playerProfile.UserId,
                ApplicationUser = playerProfile.User?.FromSharedUser() // Assuming you have a similar mapper for User
            };
        }

        public static ProjectAether.Objects.Net._2._1.Standard.Models.PlayerProfile? ToSharedPlayerProfile(this Models.PlayerProfile playerProfile)
        {
            if (playerProfile == null)
            {
                return null;
            }
            return new ProjectAether.Objects.Net._2._1.Standard.Models.PlayerProfile
            {
                Id = playerProfile.Id,
                PlayerName = playerProfile.PlayerName,
                Characters = playerProfile.Characters?.Select(c =>   c.ToSharedPlayerCharacter()).ToList(),
                UserId = playerProfile.ApplicationUserId,
                User = playerProfile.ApplicationUser?.ToSharedUser() // Assuming you have a similar mapper for User
            };
        }
    }
}
