using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FC.Framework;
using FC.Framework.Domain;
using FC.Framework.Utilities;
using DotPay.Common;
using DotPay.MainDomain.Exceptions;
using DotPay.MainDomain.Events;
namespace DotPay.MainDomain
{
    public class ScoreRecord : DomainBase, IAggregateRoot
    {
        #region ctor
        protected ScoreRecord() { }

        public ScoreRecord(int userID, int sroce, int sourceID, ScoreSource source)
        {
            this.UserID = userID;
            this.Score = sroce;
            this.SourceID = sourceID;
            this.SourceUniqueID = string.Empty;
            this.Source = source;
            this.CreateAt = DateTime.Now.ToUnixTimestamp();
        }

        public ScoreRecord(int userID, int sroce, string sourceUniqueID, ScoreSource source)
        {
            this.UserID = userID;
            this.Score = sroce;
            this.SourceID = 0;
            this.SourceUniqueID = sourceUniqueID;
            this.Source = source;
            this.CreateAt = DateTime.Now.ToUnixTimestamp();
        }
        #endregion

        #region properties
        public virtual int ID { get; protected set; }
        public virtual int UserID { get; protected set; }
        public virtual int SourceID { get; protected set; }
        public virtual string SourceUniqueID { get; protected set; }
        public virtual int Score { get; protected set; }
        public virtual ScoreSource Source { get; protected set; }
        public virtual int CreateAt { get; protected set; }
        #endregion

    }
}
