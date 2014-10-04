using FC.Framework.Repository;
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.Domain.Repository
{
    public interface IScoreRecordRepository : IRepository
    {
        ScoreRecord FindBySourceIDAndSourceType(int sourceID, ScoreSource source);

        void Remove(ScoreRecord scoreRecord);
    }
}
