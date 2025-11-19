namespace DragonGame.Services
{
    public interface ICharacterAssetsService
    {
        IEnumerable<string> GetHairOptions();
        IEnumerable<string> GetFaceOptions();
        IEnumerable<string> GetClothingOptions();
    }

    public class CharacterAssetsService : ICharacterAssetsService
    {
        public IEnumerable<string> GetHairOptions() =>
            System.IO.Directory.GetFiles("wwwroot/images/hair").Select(System.IO.Path.GetFileName);

        public IEnumerable<string> GetFaceOptions() =>
            System.IO.Directory.GetFiles("wwwroot/images/faces").Select(System.IO.Path.GetFileName);

        public IEnumerable<string> GetClothingOptions() =>
            System.IO.Directory.GetFiles("wwwroot/images/clothes").Select(System.IO.Path.GetFileName);
    }
}
