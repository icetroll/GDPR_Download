(function () {
    'use strict';

    angular.module('app').constant('dataConstants', {
        LOGIN_URL: 'http://localhost:31815/Token',
        ACCOUNT_URL: 'http://localhost:31815/api/Account/',
        ROLES_URL: 'http://localhost:31815/api/Roles/',
        SERVER_PATH: 'http://localhost:31815/',
        API_PATH: 'http://localhost:31815/api/',
    });
})();