using Agrishare.Core.Entities;
using System;

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

        public Region CurrentRegion
        {
            get
            {
                if (currentRegion == null)
                {
                    try
                    {
                        var regionId = Convert.ToInt32(Request.Cookies["region"].Value);
                        currentRegion = Region.Find(regionId);
                    }
                    catch
                    {
                        currentRegion = Region.Find(1);
                    }

                    if (currentRegion == null)
                        currentRegion = Region.Find(1);
                }

                return currentRegion;
            }
            set
            {
                currentRegion = value;
            }
        }
        private Region currentRegion;
    }
}