(function () {
    'use strict';

    angular
        .module('app')
        .factory('tokenStorage', tokenStorage);

    tokenStorage.$inject = ['$window', '$cookieStore'];

    function tokenStorage($window, $cookieStore) {

        var localStore = $window.localStorage;

        var add = function (key, value) {
            value = angular.toJson(value);
            if (localStorage) {
                localStore.setItem(key, value);
            } else {
                $cookieStore.put(key, value);
            }
        };

        var get = function (key) {
            var value = '';
            if (localStorage) {
                value = localStore.getItem(key);
                if (value) {
                    value = angular.fromJson(value);
                }
            } else {
                value = $cookieStore.get(key);
                if (value) {
                    value = angular.fromJson(value);
                }
            }
            return value;
        };

        var remove = function (key) {
            if (localStorage) {
                localStore.removeItem(key);
            } else {
                $cookieStore.remove(key);
            }
        };

        return {
            add: add,
            get: get,
            remove: remove
        };
    }
})();