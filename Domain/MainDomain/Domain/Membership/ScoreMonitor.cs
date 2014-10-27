using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework.Domain;
using FC.Framework;
using DotPay.MainDomain.Events;
using DotPay.Common;
using FC.Framework.Utilities;
using FC.Framework.Repository;
using DotPay.MainDomain.Repository;

namespace DotPay.MainDomain
{
    public class ScoreMonitor : IEventHandler<ReceiveScoreDaliyLogin>,              //用户今日首次登录
                                IEventHandler<CNYDepositCompleted>,                 //cny充值完成
                                IEventHandler<CNYDepositUndoComplete>,              //撤销cny充值
                                IEventHandler<CNYWithdrawCompleted>                //CNY提现完成    
    {
        public void Handle(ReceiveScoreDaliyLogin @event)
        {
            var repos = IoC.Resolve<IRepository>();
            var user = repos.FindById<User>(@event.UserID);

            var loginScore = 10;

            user.ScoreIncrease(loginScore);

            var scoreRecord = new ScoreRecord(@event.UserID, loginScore, 0, ScoreSource.Login);

            repos.Add(scoreRecord);
        }

        public void Handle(CNYDepositCompleted @event)
        {
            var repos = IoC.Resolve<IRepository>();
            var user = repos.FindById<User>(@event.DepositUserID);
            //RemoveBySourceIDAndSourceType
            var score = (int)((decimal)@event.DepositAmount);

            user.ScoreIncrease(score);

            var scoreRecord = new ScoreRecord(@event.DepositUserID, score, @event.DepositID, ScoreSource.CNYDeposit);

            repos.Add(scoreRecord);
        }

        public void Handle(CNYDepositUndoComplete @event)
        {
            var repos = IoC.Resolve<IRepository>();
            var scoreRepos = IoC.Resolve<IScoreRecordRepository>();
            var deposit = repos.FindById<CNYDeposit>(@event.DepositID);
            var user = repos.FindById<User>(deposit.UserID);
            var score = (int)(deposit.Amount * (decimal)Constants.SCORE_PERCENT_DEPOSIT_CNY);

            user.ScoreDecrease(score);

            var scoreRecord = scoreRepos.FindBySourceIDAndSourceType(@event.DepositID, ScoreSource.CNYDeposit);
            scoreRepos.Remove(scoreRecord);
        }

        public void Handle(CNYWithdrawCompleted @event)
        {
            var repos = IoC.Resolve<IRepository>();
            var withdraw = repos.FindById<CNYWithdraw>(@event.WithdrawID);
            var user = repos.FindById<User>(withdraw.UserID);
            //RemoveBySourceIDAndSourceType
            var score = (int)((double)withdraw.Amount);

            user.ScoreDecrease(score);

            var scoreRecord = new ScoreRecord(withdraw.UserID, score, @event.WithdrawID, ScoreSource.CNYWithdraw);

            repos.Add(scoreRecord);
        }
       
    }
}
