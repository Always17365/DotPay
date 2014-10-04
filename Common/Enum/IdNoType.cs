using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework;

namespace DotPay.Common
{
    /// <summary>
    /// the ID type
    /// </summary>
    public enum IdNoType
    {
        [EnumDescription("IdentificationCard")]
        IdentificationCard = 1,
        [EnumDescription("Passport")]
        Passport = 2,
        [EnumDescription("军官证")]
        CertificateOfOfficer = 3,
        [EnumDescription("台湾通行证")]
        TaiwanResidentsPass = 4,
        [EnumDescription("港澳通行证")]
        HKAndMacauResidentsPass = 5
    }
}
