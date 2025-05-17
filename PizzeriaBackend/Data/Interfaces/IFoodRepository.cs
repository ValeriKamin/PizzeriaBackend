using PizzeriaBackend.Models.Menu;
using System.Collections.Generic;

namespace PizzeriaBackend.Data.Interfaces
{
    public interface IFoodRepository
    {
        void UpdatePrice(int foodId, decimal newPrice);
        List<Food> GetAllFoods();
    }


}
