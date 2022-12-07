using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.Specification;
using ArrayApp.Domain.Entities.CategoryAggregate;

namespace ArrayApp.Domain.Entities.IdeaAggregate.Specifications;
public class IdeaByIdWithItemsSpec : Specification<Idea>, ISingleResultSpecification
{
    public IdeaByIdWithItemsSpec(int IdeaId)
    {
        Query
            .Where(i => i.Id == IdeaId)
            .Include(i => i.Category)
            .Include(i => i.Tags)
            .Include(i => i.Comments);
    }
}
