using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kumo_eip_06.EIP.Data.Infrastructure
{
    public class DbFactory : Disposable
    {
        KUMOEIPDataContext dbContext;

        public KUMOEIPDataContext Init()
        {
            return dbContext ?? (dbContext = new KUMOEIPDataContext());
        }

        protected override void DisposeCore()
        {
            if (dbContext != null)
                dbContext.Dispose();
        }
    }
}
