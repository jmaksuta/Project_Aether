using ProjectAether.Objects.Net._2._1.Standard.Models;

namespace Project_Aether_Backend.Shared
{
    public static class PlayerCharacterMapper
    {

        internal static Models.PlayerCharacter FromSharedPlayerCharacter(ProjectAether.Objects.Net._2._1.Standard.Models.PlayerCharacter c)
        {
            throw new NotImplementedException();
        }

        public static ProjectAether.Objects.Net._2._1.Standard.Models.PlayerCharacter? ToSharedPlayerCharacter(this Models.PlayerCharacter playerCharacter)
        {
            if (playerCharacter == null)
            {
                return null;
            }
            return new ProjectAether.Objects.Net._2._1.Standard.Models.PlayerCharacter
            {
                Id = playerCharacter.Id,
                profilePictureId = playerCharacter.profilePictureId,
                DisplayName = playerCharacter.DisplayName,
                PlayerProfileId = playerCharacter.PlayerProfileId,
                Player = playerCharacter.Player?.ToSharedPlayerProfile() // Assuming you have a similar mapper for PlayerProfile
            };
        }

    }
}
