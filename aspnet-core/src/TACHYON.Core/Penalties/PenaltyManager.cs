using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Penalties;

namespace TACHYON.Packing
{
   public class PenaltyManager : TACHYONDomainServiceBase
    {
        private readonly IRepository<Penalty> _offerRepository;
        public PenaltyManager(IRepository<Penalty> offerRepository)
        {
            _offerRepository = offerRepository;
        }
    }
}
