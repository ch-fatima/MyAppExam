using System.ComponentModel;

namespace Core.Models.Enums
{
    public enum EChannel : short
    {
        [Description("اپ موبایل")] //60d
        Mobile = 0,

        [Description("پنل ادمین")] //6h
        Panel = 1,

        [Description("لندینگ")] //None
        Landing = 2,

        [Description("Swagger")] //None
        Swagger = 3,

        [Description("PWA")] //1h
        PWA = 4
    }
}
