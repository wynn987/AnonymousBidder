using AnonymousBidder.Data.Entity;
using AnonymousBidder.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymousBidder.Data.Repository
{
    public class FilePathRepository : RepositoryBase<FilePath>
    {
        public FilePathRepository(IUnitOfWork unit)
            : base(unit)
        { }
    }
}
