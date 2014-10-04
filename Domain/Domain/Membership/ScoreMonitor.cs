using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework.Domain;
using FC.Framework;
using DotPay.Domain.Events;
using DotPay.Common;
using FC.Framework.Utilities;
using FC.Framework.Repository;
using DotPay.Domain.Repository;

namespace DotPay.Domain
{
    public class ScoreMonitor : IEventHandler<ReceiveScoreDaliyLogin>,              //用户今日首次登录
                                IEventHandler<CNYDepositCompleted>,                 //cny充值完成
                                IEventHandler<CNYDepositUndoComplete>,              //撤销cny充值
                                IEventHandler<CNYWithdrawCompleted>,                //CNY提现完成 
                                IEventHandler<VirtualCoinDepositCompleted>,         //虚拟币充值完成
                                IEventHandler<VirtualCoinWithdrawCompleted>        //虚拟币提现完成
    //IEventHandler<UserScoreUsed>                        //积分使用掉
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

        public void Handle(VirtualCoinDepositCompleted @event)
        {
            var repos = IoC.Resolve<IRepository>();
            var user = repos.FindById<User>(@event.DepositUserID);
            //RemoveBySourceIDAndSourceType
            var score = 0;

            var source = default(ScoreSource);

            switch (@event.Currency)
            {
                case CurrencyType.BTC:
                    score = (int)(@event.DepositAmount * (decimal)Constants.SCORE_PERCENT_DEPOSIT_BTC);
                    source = ScoreSource.BTCDeposit;
                    break;
                case CurrencyType.LTC:
                    score = (int)(@event.DepositAmount * (decimal)Constants.SCORE_PERCENT_DEPOSIT_LTC);
                    source = ScoreSource.LTCDeposit;
                    break;
                case CurrencyType.IFC:
                    score = (int)(@event.DepositAmount * (decimal)Constants.SCORE_PERCENT_DEPOSIT_IFC);
                    source = ScoreSource.IFCWithdraw;
                    break;
                case CurrencyType.NXT:
                    score = (int)(@event.DepositAmount * (decimal)Constants.SCORE_PERCENT_DEPOSIT_NXT);
                    source = ScoreSource.NXTDeposit;
                    break;
                case CurrencyType.DOGE:
                    score = (int)(@event.DepositAmount * (decimal)Constants.SCORE_PERCENT_DEPOSIT_DOGE);
                    source = ScoreSource.DOGEDeposit;
                    break;
                case CurrencyType.STR:
                    score = (int)(@event.DepositAmount * (decimal)Constants.SCORE_PERCENT_DEPOSIT_STR);
                    source = ScoreSource.STRDeposit;
                    break;
                case CurrencyType.FBC:
                    score = (int)(@event.DepositAmount * (decimal)Constants.SCORE_PERCENT_DEPOSIT_FBC);
                    source = ScoreSource.FBCDeposit;
                    break;
                default:
                    throw new NotImplementedException();
            }

            user.ScoreIncrease(score);
            var scoreRecord = new ScoreRecord(@event.DepositUserID, score, @event.DepositID, source);

            repos.Add(scoreRecord);
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
        public void Handle(VirtualCoinWithdrawCompleted @event)
        {
            var repos = IoC.Resolve<IRepository>();

            var withdraw = IoC.Resolve<IWithdrawRepository>().FindByIdAndCurrency(@event.WithdrawID, @event.Currency);

            var user = repos.FindById<User>(@event.WithdrawUserID);
            //RemoveBySourceIDAndSourceType
            var score = 0;

            var source = default(ScoreSource);

            switch (@event.Currency)
            {
                case CurrencyType.BTC:
                    score = (int)(withdraw.Amount * (decimal)Constants.SCORE_PERCENT_DEPOSIT_BTC);
                    source = ScoreSource.BTCDeposit;
                    break;
                case CurrencyType.LTC:
                    score = (int)(withdraw.Amount * (decimal)Constants.SCORE_PERCENT_DEPOSIT_LTC);
                    source = ScoreSource.LTCDeposit;
                    break;
                case CurrencyType.IFC:
                    score = (int)(withdraw.Amount * (decimal)Constants.SCORE_PERCENT_DEPOSIT_IFC);
                    source = ScoreSource.IFCWithdraw;
                    break;
                case CurrencyType.NXT:
                    score = (int)(withdraw.Amount * (decimal)Constants.SCORE_PERCENT_DEPOSIT_NXT);
                    source = ScoreSource.NXTDeposit;
                    break;
                case CurrencyType.DOGE:
                    score = (int)(withdraw.Amount * (decimal)Constants.SCORE_PERCENT_DEPOSIT_DOGE);
                    source = ScoreSource.DOGEDeposit;
                    break;
                case CurrencyType.STR:
                    score = (int)(withdraw.Amount * (decimal)Constants.SCORE_PERCENT_DEPOSIT_STR);
                    source = ScoreSource.STRDeposit;
                    break;
                case CurrencyType.FBC:
                    score = (int)(withdraw.Amount * (decimal)Constants.SCORE_PERCENT_DEPOSIT_FBC);
                    source = ScoreSource.FBCDeposit;
                    break;
                default:
                    throw new NotImplementedException();
            }

            user.ScoreDecrease(score);
            var scoreRecord = new ScoreRecord(withdraw.UserID, -score, @event.WithdrawID, source);

            repos.Add(scoreRecord);
        }
    }
}
