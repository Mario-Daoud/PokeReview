using PokeReview.Models;

namespace PokeReview.Interfaces
{
    public interface ICategoryRepository
    {
        ICollection<Category> GetCategories();
        Category GetCategory(int categoryId);
        ICollection<Pokemon> GetPokemonByCategory(int categoryId);
        bool CategoryExists(int idcategoryId);
        bool CreateCategory(Category category);
        bool Save();
    }
}
