/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode)
 */

agrishareApp.factory('App', function ($location, Utils) {

    var App = {
        apiUrl: '',
        user: {},
        region: {},
        regions: [],
        cookieDomain: '',

        title: '',
        status: '',
        selectedUrl: '',
        returnUrl: '',

        menu: [
            { title: "Dashboard", icon: "assessment", url: "#/dashboard", roles: 'Administrator,Dashboard' },
            { title: "Listings", icon: "local_shipping", url: "#/listings/list", roles: 'Administrator' },
            { title: "Products", icon: "shopping_basket", url: "#/products/list", roles: 'Administrator' },
            { title: "Bookings", icon: "account_balance_wallet", url: "#/bookings/list", roles: 'Administrator' },
            { title: "Ledger", icon: "receipt", url: "#/journals/list", roles: 'Administrator' },
            { title: "Adverts", icon: "panorama", url: "#/adverts/list", roles: 'Administrator' },
            { title: "Users", icon: "person", url: "#/users/list", roles: 'Administrator' },
            {
                title: "Deleted", icon: "delete_sweep", roles: 'Administrator', submenu: [
                    { title: "Listings", url: "#/deleted/listings/list", roles: 'Administrator' },
                    { title: "Users", url: "#/deleted/users/list", roles: 'Administrator' }
                ]
            },
            {
                title: "Admin", icon: "settings", roles: 'Administrator', submenu: [
                    { title: "Agents", url: "#/agents/list", roles: 'Administrator' },
                    { title: "Blog", url: "#/blogs/list", roles: 'Administrator' },
                    { title: "Broadcast SMS", url: "#/sms/broadcast", roles: 'Administrator' },
                    { title: "FAQs", url: "#/faqs/list", roles: 'Administrator' },
                    { title: "Log", url: "#/log/list", roles: 'Administrator' },
                    { title: "Regions", url: "#/regions/list", roles: 'Administrator' },
                    { title: "Settings", url: "#/settings/list", roles: 'Administrator' },
                    { title: "Suppliers", url: "#/suppliers/list", roles: 'Administrator' },
                    { title: "Tags", url: "#/tags/list", roles: 'Administrator' },
                    { title: "Templates", url: "#/templates/list", roles: 'Administrator' },
                    { title: "Transaction Fees", url: "#/transactionfees/list", roles: 'Administrator' },
                    { title: "Vouchers", url: "#/vouchers/list", roles: 'Administrator' }
                ]
            }
        ],

        authorizationHeader: function () {
            return {
                "Authorization": App.user.AuthToken,
                'UTCOffset': moment().utcOffset()
            };
        },

        hasRole: function (role) {
            return this.user.Roles.includes(role);
        },

        hasRoles: function (roles) {
            return this.user.Roles.filter(function (n) {
                return roles.indexOf(n) !== -1;
            }).length > 0;
        },

        returnUrlQs: function () {
            return 'return=' + encodeURIComponent(location.hash);
        },

        redirect: function (url) {
            location.href = url;
        },

        window: function (url, width = 400, height = 300) {
            window.open(url, "", "width=" + width + ",height=" + height);
        },

        go: function (url) {
            location.hash = url;
        },

        setRegion: function (region) {
            this.region = region;
            Utils.cookie.create('region', region.Id, 365, this.cookieDomain);
            location.reload();
        },

        slideshow: {

            visibile: false,
            title: 'Slideshow',
            currentIndex: 0,
            photos: [],

            show: function (photos, index = 0) {
                var slideshow = this;

                if (photos.length)
                    slideshow.photos = photos;
                else
                    slideshow.photos = [photos];

                slideshow.currentIndex = index;
                slideshow.visible = true;
            },

            hide: function () {
                var slideshow = this;
                slideshow.visible = false;
            },

            previous: function () {
                var slideshow = this;
                slideshow.currentIndex += 1;
                if (slideshow.currentIndex >= slideshow.photos.length)
                    slideshow.currentIndex = 0;
            },

            next: function () {
                var slideshow = this;
                slideshow.currentIndex -= 1;
                if (slideshow.currentIndex < 0)
                    slideshow.currentIndex = slideshow.photos.length - 1;
            }

        }
        
    };

    return App;
});