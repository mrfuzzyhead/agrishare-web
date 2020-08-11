using Agrishare.Core.Entities;

namespace Agrishare.CMS.Code
{
    public class BasePage : System.Web.UI.Page
    {
        public User CurrentUser
        {
            get
            {
                if (currentUser == null)
                {
                    try
                    {
                        string encryptedToken = Request.Cookies[Core.Entities.User.AuthCookieName].Value;
                        string authToken = Core.Utils.Encryption.DecryptWithRC4(encryptedToken, Config.EncryptionSalt);
                        currentUser = Core.Entities.User.Find(AuthToken: authToken);
                    }
                    catch
                    {
                        currentUser = new User();
                    }

                    if (currentUser == null)
                        currentUser = new User();
                }

                return currentUser;
            }
            set
            {
                currentUser = value;
            }
        }
        private User currentUser;
    }
}