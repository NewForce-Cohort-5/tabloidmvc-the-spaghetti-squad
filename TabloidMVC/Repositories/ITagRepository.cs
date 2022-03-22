using System.Collections.Generic;
using TabloidMVC.Models;

namespace TabloidMVC.Repositories
{
    public interface ITagRepository
    {
        List<Tag> GetAllTags();

    Tag GetTagById(int id);
     void DeleteTag(int TagId);

     void AddTag(Tag tag);

     void UpdateTag(Tag tag, int id);

    }
}
