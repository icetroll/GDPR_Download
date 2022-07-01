(function () {
    'use strict';

    angular
        .module('app')
        .factory('userAccount', ['$resource', 'dataConstants', userAccount]);

    function userAccount($resource, dataConstants) {

        return {
            confirmterms: $resource(dataConstants.ACCOUNT_URL + 'confirmterms', null,
                {
                    'confirmterms': { method: 'POST' }
                }),
            getUserGeneralInfo: $resource(dataConstants.ACCOUNT_URL + 'getusergeneralinfo', null,
                {
                    'getUserGeneralInfo': {
                        method: 'GET'
                    }
                }),
            getRoles: $resource(dataConstants.ROLES_URL + 'GetAllRoles', null,
                {
                    'getRoles':
                        {
                            method: 'GET',
                            isArray: true
                        }
                }),
            getUsers: $resource(dataConstants.GETUSERS_URL, null,
                {
                    'getUsers': { method: 'GET' }
                }),
            getUserPermission: $resource(dataConstants.ROLES_URL + 'getuserpermission', null,
                {
                    'getUserPermission':
                        {
                            method: 'GET'
                        }
                }),
            getUserbyUserName: $resource(dataConstants.ACCOUNT_URL + 'getuserbyusername/username', null,
                {
                    'getUserbyUserName':
                        {
                            method: 'GET',
                            username: '@username'
                        }
                }),
            registration: $resource(dataConstants.ACCOUNT_URL + 'register', null,
                {
                    'registerUser': { method: 'POST' }
                }),
            updateUser: $resource(dataConstants.ACCOUNT_URL + 'updateuserinformation', null,
                {
                    'updateUser': { method: 'POST' }
                }),
            deleteUser: $resource(dataConstants.ACCOUNT_URL + 'deleteuser/username', null,
                {
                    'deleteUser': {
                        method: 'DELETE',
                        username: '@username',
                        headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
                    }
                }),
            changePassword: $resource(dataConstants.ACCOUNT_URL + 'changepassword', null,
                {
                    'changePassword': {
                        method: 'POST',
                        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                        transformRequest: function (data, headersGetter) {
                            var str = [];
                            for (var d in data) {
                                str.push(encodeURIComponent(d) + "=" + encodeURIComponent(data[d]));
                            }
                            return str.join('&').replace(/%20/g, '+');
                        }
                    }
                }),
            login: $resource(dataConstants.LOGIN_URL, null,
                {
                    'loginUser': {
                        method: 'POST',
                        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                        transformRequest: function (data, headersGetter) {
                            var str = [];
                            for (var d in data) {
                                str.push(encodeURIComponent(d) + "=" + encodeURIComponent(data[d]));
                            }
                            return str.join('&').replace(/%20/g, '+');
                        }
                    }
                })
        };
    }
})();