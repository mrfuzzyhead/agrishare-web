/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode)
 */

agrishareApp.controller('ListingsMapController', function ($attrs, $controller, $document, $http, $location, $q, $scope, $timeout, App, Utils) {

    $controller('ViewController', { $scope: $scope, $attrs: $attrs });

    /* After the map is panned/zoomed, get the lat/lng for the center and the current zoom
     * Find all listings within a specified radius of the centre (use the zoom value to determine the radius)
     * Clear all markers before adding new markers
     * Tapping on a marker should bring up title with option to tap through for full details
     */

    var map = this;
    map.searching = false;
    map.query = '';
    map.gMap = null;
    map.aborter = null;
    map.delay = null;
    map.markers = [];
    map.bounds = null;
    map.categories = [1, 2, 3, 17];

    App.status = "Done";

    map.toggleOn = function (id) {
        map.categories.push(id);
        map.getListings();
    };

    map.toggleOff = function (id) {
        var index = map.categories.indexOf(id);
        map.categories.splice(index, 1);
        map.getListings();
    };

    map.getListings = function () {
        var bounds = map.bounds;

        map.aborter = $q.defer();
        $http({
            timeout: map.aborter.promise,
            url: App.apiUrl + '/cms/listings/map?NELat=' + bounds.Za.j + '&NELng=' + bounds.Ua.j + '&SWLat=' + bounds.Za.i + '&SWLng=' + bounds.Ua.i + '&' + map.categories.map(x => 'categoryId=' + x).join('&'),
            headers: App.authorizationHeader()
        }).then(function (response) {

            for (var i = 0; i < map.markers.length; i++) {
                map.markers[i].setMap(null);
            }
            map.markers = [];

            for (var j = 0; j < response.data.length; j++) {
                var listing = response.data[j];
                var marker = new google.maps.Marker({
                    position: {
                        lat: listing.Latitude,
                        lng: listing.Longitude
                    },
                    icon: '/Resources/Images/Marker-' + listing.Category.Id + '.png',
                    map: map.gMap,
                    title: listing.Title,
                    id: listing.Id
                });
                marker.addListener('click', function () {
                    App.go('/listings/detail/' + this.id + '?' + App.returnUrlQs());
                });
                map.markers.push(marker);
            }

        }, function (response) {

            App.status = 'Error loading list';

            var error = response.data && response.data.Message ? response.data.Message : 'An unknown error occurred';
            console.log(error);

        });
    };

    setTimeout(function () {

        var el = $document.find('#listingsmap').get(0);

        map.gMap = new google.maps.Map(el, {
            zoom: 6,
            center: { lat: -19.0955342, lng: 29.5124486 }
        });

        map.gMap.addListener('bounds_changed', function (e) {

            $timeout.cancel(map.delay);
            map.delay = $timeout(function () {
                map.bounds = map.gMap.getBounds();
                map.getListings();
            }, 500);

        });

    }, 500);

    $scope.map = map;

});