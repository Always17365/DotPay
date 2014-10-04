using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DotPay.Web
{
    public static class QRHelper
    {
        /// <summary>
        /// Generates an img tag with a data uri encoded image of the QR code from the content given.
        /// </summary>
        /// <param name="html"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static IHtmlString QRCode(this HtmlHelper html, string content)
        {
            QrEncoder enc = new QrEncoder(ErrorCorrectionLevel.H);
            var code = enc.Encode(content);

            GraphicsRenderer r = new GraphicsRenderer(new FixedModuleSize(5, QuietZoneModules.Two), Brushes.Black, Brushes.White);

            using (MemoryStream ms = new MemoryStream())
            {
                r.WriteToStream(code.Matrix, ImageFormat.Png, ms);

                byte[] image = ms.ToArray();
                //otpauth://totp/MY_APP_LABEL?secret={0}
                return html.Raw(string.Format(@"<img src=""data:image/png;base64,{0}"" alt=""{1}"" />", Convert.ToBase64String(image), content));
            }
        }
    }
}