using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
﻿using Dotpay.Actor.Interfaces.Tools;
﻿using Dotpay.Common.Enum;
﻿using Orleans;
﻿using Orleans.Concurrency;

namespace Dotpay.Actors.Implementations.Actors.Tools
{
    public class SequenceNoGenerator : Orleans.Grain, ISequenceNoGenerator
    {
        private const int MaxSeed = 99999;
        private int _seed = 0;
        private int _currentSecondCounter = 0;
        Task<Immutable<string>> ISequenceNoGenerator.GetNext()
        {
            var oldCounter = this._currentSecondCounter;

            while (true)
            {
                this.UpdateCurrentTimestamp();

                if (oldCounter != this._currentSecondCounter)
                    this._seed = 1;
                else
                {
                    ++this._seed;
                }

                if (this._seed > MaxSeed) Task.Delay(100);
                else break;
            }

            var seq = DateTime.Now.ToString("yyyyMMdd") +
                this.GetPrimaryKeyLong().ToString().PadLeft(3,'0')+ 
                this._currentSecondCounter.ToString().PadLeft(5,'0') + 
                this._seed.ToString().PadLeft(4, '0');
            return Task.FromResult(seq.AsImmutable());
        }

        public override Task OnActivateAsync()
        {
            //延迟1秒启动，防止Activation在某个机器上崩溃后，在集群中其它host上启动时，sequenceNo在同一秒出现重复
            Task.Delay(1000);

            return base.OnActivateAsync();
        }

        private void UpdateCurrentTimestamp()
        {
            var currentTime = DateTime.Now;
            var currentDayStart = Convert.ToDateTime(currentTime.ToShortDateString());
            this._currentSecondCounter = (int)(new TimeSpan(currentTime.Ticks - currentDayStart.Ticks).TotalSeconds);
        }
    }
}
