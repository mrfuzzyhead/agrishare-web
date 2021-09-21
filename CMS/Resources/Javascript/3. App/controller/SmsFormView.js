/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode)
 */

agrishareApp.controller('SmsFormViewController', function ($attrs, $controller, $http, $location, $rootScope, $scope, App, Utils) {

    $controller('FormViewController', { $scope: $scope, $attrs: $attrs });

    $scope.$watch('form.entity', function (entity, old) {

        if (entity && old) {
            if (entity.FilterView !== old.FilterView || entity.CategoryId !== old.CategoryId) {

                $scope.form.entity.RecipientCount = null;

                $http({
                    url: $scope.form.apiUrl + 'recipients/count?view=' + entity.FilterView + '&categoryid=' + entity.CategoryId,
                    headers: App.authorizationHeader()
                })
                    .then(function (response) {
                        $scope.form.entity.RecipientCount = response.data.Entity.RecipientCount;
                    }, function (response) {
                        Utils.alert('Error', response.data.Message || 'An unknown error occurred');
                    });

            }
        }

    }, true);

});