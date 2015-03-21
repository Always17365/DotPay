using System;
using System.Threading.Tasks;
using Dotpay.Actor.Interfaces;
using Dotpay.Actor.Implementations;
using Dotpay.Common;
using Orleans;
using Orleans.Runtime;
using RabbitMQ.Client;

namespace Dotpay.Actor.Service.Implementations
{
    public abstract class MessageConsumerAbstractGrain : Grain, IRemindable
    {
        private const string CheckSelfReminderName = "__CheckSelfAndDeactivate";
        private DateTime _lastConsumeMessageAt;
        protected bool started;
        protected IModel channel;

        public virtual Task Start()
        {
            if (!this.started) this.started = true;

            return TaskDone.Done;
        }

        protected abstract Task<bool> CheckSelfCanBeDeactive();
        protected abstract Task StartMessageConsumer();

        public async override Task OnActivateAsync()
        {
            this._lastConsumeMessageAt = DateTime.Now;
            base.DelayDeactivation(TimeSpan.FromDays(365 * 10));
            var timeSpan = TimeSpan.FromMinutes(Constants.AtleastOnActivationInSiloGrainSelfCheckPeriod);

            this.channel = RabbitMqConnectionManager.GetConnection().CreateModel();
            await this.StartMessageConsumer();
            await base.RegisterOrUpdateReminder(CheckSelfReminderName, timeSpan, timeSpan);
            await base.OnActivateAsync();
        }

        public override Task OnDeactivateAsync()
        {
            if (this.channel.IsOpen) this.channel.Close();
            else
            {
                try { this.channel.Dispose(); }
                catch (Exception) { }
            }
            return base.OnDeactivateAsync();
        }

        public async virtual Task ReceiveReminder(string reminderName, TickStatus status)
        {
            if (reminderName.Equals(CheckSelfReminderName))
            {
                var expired =
                    this._lastConsumeMessageAt.AddMinutes(Constants.AtleastOnActivationInSiloGrainSelfCheckPeriod) <
                    DateTime.Now;

                if (expired && await CheckSelfCanBeDeactive())
                {
                    await this.UnregisterReminder(await this.GetReminder(reminderName));
                    this.DeactivateOnIdle();
                }
            }
        }

        public virtual Task Receive(MqMessage message)
        {
            this._lastConsumeMessageAt = DateTime.Now;
            return TaskDone.Done;
        }
    }
}
