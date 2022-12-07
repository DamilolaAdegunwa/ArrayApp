using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.Specification;

namespace ArrayApp.Domain.Entities.IdeaAggregate.Specifications;
public class IdeaWithItemsSpec : Specification<Idea>, ISingleResultSpecification
{
    public IdeaWithItemsSpec()
    {
        Query
            .Include(i => i.Category)
            .Include(i => i.Tags)
            .Include(i => i.Comments);
    }
}
