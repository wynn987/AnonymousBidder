﻿using AnonymousBidder.Data.Entity;
using AnonymousBidder.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymousBidder.Data.Repository
{
    public class AuctionRepository : RepositoryBase<Auction>
    {
        public AuctionRepository(IUnitOfWork unit)
            : base(unit)
        { }
    }
}
