using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text; 
using Dotpay.Actor.Tools.Interfaces;
using Dotpay.Common.Enum;
﻿using Orleans;
﻿using Orleans.Concurrency;

namespace Dotpay.Actor.Tools.Implementations
{
    /// <summary>
    /// 序号生成器
    /// <remarks>高性能序号生成器,非连续性、但有序的序号生成器</remarks>
    /// </summary>
    public class SequenceNoGenerator : Orleans.Grain, ISequenceNoGenerator
    {
        private const int MaxSeed = 99999;
        private int _seed = 0;
        private int _currentTimestampSecondCounter = 0;
        Task<string> ISequenceNoGenerator.GetNext()
        {
            var oldCounter = this._currentTimestampSecondCounter;

            while (true)
            {
                this.UpdateCurrentTimestamp();

                if (oldCounter != this._currentTimestampSecondCounter)
                    this._seed = 1;
                else
                {
                    ++this._seed;
                }

                if (this._seed > MaxSeed) Task.Delay(100);
                else break;
            }

            //设计中，这个long key前三位是SequenceNoType大类,后两位是小类,一共为5位
            var sequeneceTypeString = this.GetPrimaryKeyLong().ToString().PadLeft(5, '0');

            //生成的序号示例:2015031510012000112359
            var seq = DateTime.Now.ToString("yyyyMMdd") + sequeneceTypeString +
                      this._seed.ToString().PadLeft(4, '0') +
                      this._currentTimestampSecondCounter.ToString().PadLeft(5, '0');
            return Task.FromResult(seq);
        }

        public override Task OnActivateAsync()
        {
            //延迟1秒启动，防止Activation在某个机器上崩溃后，在集群中其它host上启动时，sequenceNo在同一秒出现重复
            Task.Delay(1000);
            base.DelayDeactivation(TimeSpan.FromDays(365 * 10));//seq不应被回收
            return base.OnActivateAsync();
        }

        private void UpdateCurrentTimestamp()
        {
            var currentTime = DateTime.Now;
            var currentDayStart = Convert.ToDateTime(currentTime.ToShortDateString());
            this._currentTimestampSecondCounter = (int)(new TimeSpan(currentTime.Ticks - currentDayStart.Ticks).TotalSeconds);
        }
    }
}
