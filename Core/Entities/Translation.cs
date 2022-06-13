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
        private static readonly List<Translation> List = new List<Translation>
        {
            new Translation { Key = TranslationKey.VerificationCode.ToString(), Language = Language.English, Text = "Your verification code is {0}" },
            new Translation { Key = TranslationKey.BookingCancelled.ToString(), Language = Language.English, Text = "Booking {0} cancelled" },
            new Translation { Key = TranslationKey.BookingConfirmed.ToString(), Language = Language.English, Text = "Booking {0} confirmed" },
            new Translation { Key = TranslationKey.BookingReceived.ToString(), Language = Language.English, Text = "New booking received: {0}" },
            new Translation { Key = TranslationKey.NewReview.ToString(), Language = Language.English, Text = "New review received for booking {0}" },
            new Translation { Key = TranslationKey.PaymentReceived.ToString(), Language = Language.English, Text = "Payment received for booking {0}" },
            new Translation { Key = TranslationKey.BookingCompleted.ToString(), Language = Language.English, Text = "Booking {0} completed" },
            new Translation { Key = TranslationKey.BookingNotCompleted.ToString(), Language = Language.English, Text = "Booking {0} not completed" }
        };

        public static string Translate(TranslationKey Key, Language Language)
        {
            var phrase = List.FirstOrDefault(o => o.Key == Key.ToString() && o.Language == Language);

            if (phrase == null)
                phrase = List.FirstOrDefault(o => o.Key == Key.ToString() && o.Language == Language.English);

            if (phrase != null)
                return phrase.Text;

            return string.Empty;
        }
    }

    public class Translation
    {
        public Language Language { get; set; }
        public string Key { get; set; }
        public string Text { get; set; }
    }
}
