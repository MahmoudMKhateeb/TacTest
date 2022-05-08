using Abp.Runtime.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Documents.DocumentFiles.Dtos;
using TACHYON.Dto;

namespace TACHYON.Storage
{
    public class TempFileCacheManager : ITempFileCacheManager
    {
        public const string TempFileCacheName = "TempFileCacheName";

        private readonly ICacheManager _cacheManager;

        public TempFileCacheManager(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public void SetFile(string token, byte[] content)
        {
            _cacheManager.GetCache(TempFileCacheName)
                .Set(token, content, new TimeSpan(0, 0, 20, 0)); // expire time is 1 min by default
        }

        public void ClearCache(string token)
        {
            _cacheManager.GetCache(TempFileCacheName).Remove(token);
        }
        public byte[] GetFile(string token)
        {
            return _cacheManager.GetCache(TempFileCacheName).Get(token, ep => ep) as byte[];
        }

    }
}