﻿/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode)
 */

var agrishareApp =
    angular
        .module('agrishareApp', ['ui.router', '720kb.tooltips', 'ngToast', 'ngAnimate', 'ngFileUpload', 'cp.ngConfirm', 'ngMap', 'angular-svg-round-progressbar', 'chart.js'])
        .config(function ($sceDelegateProvider, ngToastProvider, $stateProvider, $locationProvider, $urlRouterProvider) {

            $sceDelegateProvider.resourceUrlWhitelist([
                'self'
            ]);

            ngToastProvider.configure({
                animation: 'slide',
                horizontalPosition: 'right',
                verticalPosition: 'bottom',
                combineDuplications: true
            });

            $stateProvider
                .state('Default', {
                    url: '/{path:[a-z\/]+}',
                    templateUrl: function ($stateParams) {
                        return '/Pages/' + $stateParams.path + '.html';
                    }
                })
                .state('Filter', {
                    url: '/{path:[a-z\/]+}/filter/{filter:.*}',
                    templateUrl: function ($stateParams) {
                        return '/Pages/' + $stateParams.path + '.html';
                    },
                    controller: function ($scope, $stateParams) {
                        $scope.filter = {};
                        var parts = $stateParams.filter.split('/');
                        for (var i = 0; i < parts.length; i += 2) {
                            if (parts[i].match(/date/)) {
                                var dt = moment(parts[i + 1]);
                                $scope.filter[parts[i]] = dt.toDate();
                            }
                            else {
                                var num = parseInt(parts[i + 1]);
                                if (isNaN(num))
                                    $scope.filter[parts[i]] = parts[i + 1];
                                else
                                    $scope.filter[parts[i]] = num;
                            }
                        }
                    }
                })
                .state('Detail', {
                    url: '/{path:[a-z\/]+}/{id:[0-9]+}',
                    templateUrl: function ($stateParams) {
                        return '/Pages/' + $stateParams.path + '.html';
                    },
                    controller: function ($scope, $stateParams) {
                        $scope.entityId = $stateParams.id;
                    }
                });
            $urlRouterProvider.otherwise('/dashboard');

            // trust html from local resources
            $sceDelegateProvider.resourceUrlWhitelist([
                'self'
            ]);

        })
        .filter('trustAsHtml', function ($sce) {
            return $sce.trustAsHtml;
        })
        .filter('trustAsResourceUrl', ['$sce', function ($sce) {
            return function (val) {
                return $sce.trustAsResourceUrl(val);
            };
        }])
        .filter('bytes', function () {
            return function (bytes, precision) {
                if (bytes === 0 || isNaN(parseFloat(bytes)) || !isFinite(bytes)) return '-';
                if (typeof precision === 'undefined') precision = 1;
                var units = ['bytes', 'KB', 'MB', 'GB', 'TB', 'PB'],
                    number = Math.floor(Math.log(bytes) / Math.log(1024));
                return (bytes / Math.pow(1024, Math.floor(number))).toFixed(precision) + ' ' + units[number];
            };
        })
        .filter('percent', function () {
            return function (value, precision) {
                if (isNaN(parseFloat(value)) || !isFinite(value)) return '-';
                if (typeof precision === 'undefined') precision = 1;
                return (value * 100).toFixed(precision) + '%';
            };
        })
        .filter('newLines', function () {
            return function (value) {
                try {
                    return value.replace(/(?:\r\n|\r|\n)/gi, '<br/>');
                }
                catch (ex) {
                    return value;
                }
            };
        })
        .directive('glEnterKeypress', function () {
            return function (scope, element, attrs) {
                element.bind("keydown keypress", function (event) {
                    if (event.which === 13) {
                        scope.$apply(function () {
                            scope.$eval(attrs.glEnterKeypress);
                        });
                        event.preventDefault();
                    }
                });
            };
        })
        .directive('glRightClick', function ($parse) {
            return function (scope, element, attrs) {
                var fn = $parse(attrs.ngRightClick);
                element.bind('contextmenu', function (event) {
                    scope.$apply(function () {
                        event.preventDefault();
                        fn(scope, { $event: event });
                    });
                });
            };
        })
        .config(["$httpProvider", function ($httpProvider) {

            $httpProvider.defaults.withCredentials = true;

            //var jsonDateRegex = /^[\d]{4}-[\d]{2}-[\d]{2}T[\d]{2}:[\d]{2}:[\d]{2}(.[\d]+\+[\d]{2}:[\d]{2})?$/;
            var jsonDateRegex = /^[\d]{4}-[\d]{2}-[\d]{2}T[\d]{2}:[\d]{2}:[\d]{2}(.[\d]+)?(\+[\d]{2}:[\d]{2})?([Z])?$/;

            var convertDateStringsToDates = function (input) {
                if (typeof input !== "object") return input;
                for (var key in input) {
                    if (!input.hasOwnProperty(key)) continue;
                    var value = input[key];
                    var match;
                    if (typeof value === "string" && (match = value.match(jsonDateRegex))) {
                        var milliseconds = Date.parse(match[0]);
                        if (!isNaN(milliseconds)) {
                            input[key] = new Date(milliseconds);
                        }
                    } else if (typeof value === "object") {
                        convertDateStringsToDates(value);
                    }
                }
            };

            $httpProvider.defaults.transformResponse.push(function (responseData) {
                convertDateStringsToDates(responseData);
                return responseData;
            });

        }]);

