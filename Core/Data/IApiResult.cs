using System.Collections.Generic;

namespace Core.Data
{
    public interface IApiResult<T>
    {
        IEnumerable<T> GetApiResult();
        void SetResult(IEnumerable<T> list);
    }
}