using VizsgaRemekBackend.Dtos;


namespace VizsgaRemekBackend.Services
{
    public interface IFoodService
    {
        public List<AllFoodDto> GetAllFood();
    }
}
