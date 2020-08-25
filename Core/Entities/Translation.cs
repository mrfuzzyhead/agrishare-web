using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrishare.Core.Entities
{
    public enum TranslationKey
    {
        VerificationCode,
        BookingCancelled,
        BookingConfirmed,
        BookingReceived,
        NewReview,
        PaymentReceived,
        BookingCompleted,
        BookingNotCompleted
    }

    public class Translations
    {
        private static List<Translation> List = new List<Translation>
        {
            new Translation { Key = TranslationKey.VerificationCode, English = "Your verification code is {0}", Shona = "Your verification code is {0}", Ndebele = "Your verification code is {0}" },
            new Translation { Key = TranslationKey.BookingCancelled, English = "Booking {0} cancelled", Shona = "Booking {0} cancelled", Ndebele = "Booking {0} cancelled" },
            new Translation { Key = TranslationKey.BookingConfirmed, English = "Booking {0} confirmed", Shona = "Booking {0} confirmed", Ndebele = "Booking {0} confirmed" },
            new Translation { Key = TranslationKey.BookingReceived, English = "New booking received: {0}", Shona = "New booking received: {0}", Ndebele = "New booking received: {0}" },
            new Translation { Key = TranslationKey.NewReview, English = "New review received for booking {0}", Shona = "New review received for booking {0}", Ndebele = "New review received for booking {0}" },
            new Translation { Key = TranslationKey.PaymentReceived, English = "Payment received for booking {0}", Shona = "Payment received for booking {0}", Ndebele = "Payment received for booking {0}" },
            new Translation { Key = TranslationKey.BookingCompleted, English = "Booking {0} completed", Shona = "Booking {0} completed", Ndebele = "Booking {0} completed" },
            new Translation { Key = TranslationKey.BookingNotCompleted, English = "Booking {0} not completed", Shona = "Booking {0} not completed", Ndebele = "Booking {0} not completed" },
        };

        public static string Translate(TranslationKey Key, Language Language)
        {
            var phrase = List.FirstOrDefault(o => o.Key == Key);

            if (phrase != null)
                switch (Language)
                {
                    case Language.English:
                        return phrase.English;
                    case Language.Shona:
                        return phrase.Shona;
                    case Language.Ndebele:
                        return phrase.Ndebele;
                    default:
                        return phrase.English;
                }

            return string.Empty;
        }
    }

    public class Translation
    {
        public TranslationKey Key { get; set; }
        public string English { get; set; }
        public string Shona { get; set; }
        public string Ndebele { get; set; }
    }
}
