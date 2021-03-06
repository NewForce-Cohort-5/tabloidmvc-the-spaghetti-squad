using System.Collections.Generic;
using TabloidMVC.Models;

namespace TabloidMVC.Repositories
{
    public interface ICategoryRepository
    {
        List<Category> GetAll();
        public void Add(Category category);
        public void Update(Category category);
        public void Delete(int categoryId);
        public Category GetCategoryById(int id);
    }
}