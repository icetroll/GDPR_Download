(function () {
    'use strict';

    var controllerId = 'TopNavigationController';
    var supportMail = {};

    angular
        .module('app')
        .controller(controllerId, topNavigationController);

    topNavigationController.$inject = ['$location', 'currentUser', 'userAccount', 'swangular', '$scope', '$http', 'dataConstants'];

    function topNavigationController($location, currentUser, userAccount, swangular, $scope, $http, dataConstants) {
        var vm = this;
        vm.title = 'topNavigationController';
        $scope.location = $location;
        vm.userData = {
            userName: '',
            email: '',
            password: ''
        };

        $scope.$watch('location.search()', function() {        
            $scope.loginToken = ($location.search()).loginToken;
            if($scope.loginToken != undefined)
                Login($scope.loginToken);

        }, true);

        vm.isLoggedIn = function () {
            if (currentUser.getProfile().isLoggedIn) {
                return true;
            } else {
                return false;
            }
        };

        vm.userName = function () {
            return currentUser.getProfile().username;
        };

        vm.userAdmin = function () {
            var role = currentUser.getProfile().roles;
            
            if(role !== undefined)
            {
                if (role.length !== undefined) {
                    if (role.includes('Admin')) {
                        return false;
                    } else {
                        return true;
                    }
                } else {
                    return true;
                }
            }else{
                return true
            }
        }

        vm.isGuest = function () {
            var role = currentUser.getProfile().roles;
            if(role !== undefined)
            {
                if (role.length !== undefined) {
                    if (role.includes('Gäst')) {
                        return true;
                    } else {
                        return false;
                    }
                } else {
                    return true;
                }
            }else{
                return true
            }
        }

        vm.login = function () {
            swangular.enableLoading('Auto close alert!', 'Test');
            vm.userData.grant_type = "password";
            vm.userData.userName = vm.userData.email;
            swangular.swal({
                title: 'Loggar in',
                html: 'Var vänlig att vänta',
                onOpen: () => {
                    swal.showLoading();
                    userAccount.login.loginUser(vm.userData,
                        function (data) {
                            vm.message = '';
                            vm.password = '';
                            swangular.success('Din fil har nu skickats', { timer: 1 });
                            currentUser.setProfile(data.roles, vm.userData.userName, data.access_token);
                        }, function (response) {
                            vm.password = '';
                            swangular.alert("Felaktigt användarnamn eller lösenord.Vänligen försök igen.", { timer: 4000 });
                        });
                }
            });
        };

        vm.logout = function () {
            currentUser.removeProfile();
            currentUser.getProfile();
        };

        function Login(loginToken) {
            var listUrl = dataConstants.API_PATH + 'Account/GuestLogin?loginToken=' + loginToken;
            $http({
                method: 'GET',
                url: listUrl,
            }).then(function successCallback(response) {
                console.log(response);
                currentUser.setProfile(response.data.roles, response.data.userName, response.data.access_token);
            }, function errorCallback(response) {
                if (response.status === 401) {
                    swangular.alert('Du måste logga in igen');
                    vm.sent = false;
                    currentUser.removeProfile();
                    currentUser.getProfile();
                } else if(response.status === 400)
                {
                    swangular.alert('Misslyckades med att logga in');
                    vm.sent = false;
                    currentUser.removeProfile();
                    currentUser.getProfile();
                }
            });
        }
    }
})();
