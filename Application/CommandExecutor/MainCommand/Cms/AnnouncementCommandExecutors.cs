using FC.Framework;
using FC.Framework.Repository;
using FC.Framework.Utilities;
using DotPay.Common;
using DotPay.MainDomain;
using DotPay.MainDomain.Exceptions;
using DotPay.MainDomain.Repository;
using DotPay.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotPay.Command.Executor
{
    public class AnnouncementCommandExecutors : ICommandExecutor<CreateAnnouncement>,
                                                ICommandExecutor<EditAnnouncement>,
                                                ICommandExecutor<AnnouncementSignTop>,
                                                ICommandExecutor<AnnouncementUnsignTop>
    {
        public void Execute(CreateAnnouncement cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var announcement = new Announcement(cmd.Title, cmd.Content, cmd.IsTop, cmd.Lang, cmd.CreateBy);

            IoC.Resolve<IRepository>().Add(announcement);
        }

        public void Execute(EditAnnouncement cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var announcement = IoC.Resolve<IRepository>().FindById<Announcement>(cmd.AnnouncementID);

            announcement.Edit(cmd.Title, cmd.Content, cmd.IsTop, cmd.Lang, cmd.EditBy);
        }

        public void Execute(AnnouncementSignTop cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var announcement = IoC.Resolve<IRepository>().FindById<Announcement>(cmd.AnnouncementID);

            announcement.SignTop(cmd.ByUserID);
        }

        public void Execute(AnnouncementUnsignTop cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var announcement = IoC.Resolve<IRepository>().FindById<Announcement>(cmd.AnnouncementID);

            announcement.UnsignTop(cmd.ByUserID);
        }
    }
}
