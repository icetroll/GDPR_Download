(function () {
    'use strict';

    var commonModule = angular.module('common', []);

    commonModule.provider('commonConfig', function () {
        this.config = {

        };

        this.$get = function () {
            return {
                config: this.config
            };
        };
    });

    commonModule.factory('common', ['$q', '$rootScope', '$timeout', 'commonConfig', common]);

    function common($q, $rootScope, $timeout, commonConfig) {

        var service = {
            $broadcast: $broadcast,
            $q: $q,
            $timeout: $timeout,
            $rootScope: $rootScope,
            activateController: activateController
        };

        return service;

        function activateController(promises, controllerId) {

        }
    }

    function $broadcast() {
        return $rootScope.$broadcast.apply($rootScope, arguments);
    }

})();