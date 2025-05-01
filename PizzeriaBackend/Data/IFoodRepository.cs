using PizzeriaBackend.Models;
using System.Collections.Generic;

namespace PizzeriaBackend.Data
{
    public interface IFoodRepository
    {
        void UpdatePrice(int foodId, decimal newPrice);
        List<Food> GetAllFoods();
    }


}
