﻿using Abp.Dependency;
using Abp.Domain.Repositories;
using System;
using System.Threading.Tasks;

namespace TACHYON.Storage
{
    public class DbBinaryObjectManager : TACHYONDomainServiceBase, IBinaryObjectManager, ITransientDependency
    {
        private readonly IRepository<BinaryObject, Guid> _binaryObjectRepository;

        public DbBinaryObjectManager(IRepository<BinaryObject, Guid> binaryObjectRepository)
        {
            _binaryObjectRepository = binaryObjectRepository;
        }

        public Task<BinaryObject> GetOrNullAsync(Guid id)
        {
            DisableTenancyFilters();
            return _binaryObjectRepository.FirstOrDefaultAsync(id);
        }

        public Task SaveAsync(BinaryObject file)
        {
            return _binaryObjectRepository.InsertAsync(file);
        }

        public Task DeleteAsync(Guid id)
        {
            return _binaryObjectRepository.DeleteAsync(id);
        }
    }
}