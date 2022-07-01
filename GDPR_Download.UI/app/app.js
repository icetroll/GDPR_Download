(function () {
    'use strict';

    var app = angular.module('app', [

        // Angular modules 
        'ngAnimate',
        'ngRoute',
        'ngSanitize',
        'ngResource',
        'ngCookies',
        'ui.bootstrap',
        'ui.utils',
        'swangular',

        // Custom modules 
        'common'
    ]);

    app.run(['$route', '$rootScope', '$q', function ($route, $rootScope, $q) {

    }]);
})();