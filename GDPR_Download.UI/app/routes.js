(function () {
    'use strict';

    var app = angular.module('app');
    app.constant('routes', getRoutes());
    app.config(['$routeProvider', 'routes', routeConfigurator]);

    function routeConfigurator($routeProvider, routes) {

        routes.forEach(function (r) {
            setRoute(r.url, r.config);
        });

        $routeProvider.otherwise({ redirectTo: '/home' });

        function setRoute(url, definition) {
            definition.resolve = angular.extend(definition.resolve || {});
            $routeProvider.when(url, definition);
        }
    }

    function getRoutes() {
        return [{
            url: '/home',
            config: {
                templateUrl: 'app/upload/upload.html',
                controller: 'UploadController',
                controllerAs: 'vm',
                title: 'Home'
            }
        }, {
            url: '/recive',
            config: {
                templateUrl: 'app/recive/recive.html',
                controller: 'ReciveController',
                controllerAs: 'vm',
                title: 'Recive'
            }
        }, {
            url: '/login',
            config: {
                templateUrl: 'app/login/login.html',
                //controller: 'LoginController',
                //controllerAs: 'vm',
                title: 'login'
            }
        }, {
            url: '/administration',
            config: {
                templateUrl: 'app/admin/admin.html',
                controller: 'AdminController',
                controllerAs: 'vm',
                title: 'Administration'
            }

        }
        ];

    }
})();