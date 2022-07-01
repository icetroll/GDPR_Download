(function () {
    'use strict';

    var controllerId = 'AdminController';

    angular
        .module('app')
        .controller(controllerId, ['$scope', '$http', '$uibModal', 'userAccount', 'currentUser', 'swangular', 'dataConstants', adminController]);

    function adminController($scope, http, $modal, userAccount, currentUser, swangular, dataConstants) {
        var vm = this;
        vm.title = 'AdminController';
        vm.total = 0;
        $scope.showModal = false;
        $scope.hideUpdate = true;
        $scope.hideNewUser = true;

        vm.userData = {
            name: '',
            email: '',
            oldEmail: '',
            password: '',
            confirmPassword: '',
            admin: false,
            user: false
        };


        vm.settings = {
            sqlHost: '',
            sqlPort: '',
            sqlDatabase: '',
            sqlUsername: '',
            sqlPassword: '',
            smtpHost: '',
            smtpPort: '',
            smtpUsername: '',
            smtpPassword: ''
        };

        activate();
        var config = {
            headers: {
                'Authorization': currentUser.getProfile().token
            }
        }

        vm.updateUser = function (user) {
            vm.userData.name = user.name;
            vm.userData.email = user.email;
            vm.userData.oldEmail = user.email;
            if (user.roles.includes('Administratör')) {
                vm.userData.admin = true;
            }
            else {
                vm.userData.admin = false;
            }

            if (user.roles.includes('Användare')) {
                vm.userData.user = true;
            }
            else {
                vm.userData.user = false;
            }
            $scope.hideUpdate = false;
            $scope.hideNewUser = true;
        }

        vm.removeUser = function (email) {
            var listUrl = dataConstants.API_PATH + 'Admin/RemoveUser?email=' + email;
            http({
                method: 'POST',
                url: listUrl,
                data: '?email=' + email,
                headers: {
                    Authorization: 'Bearer ' + currentUser.getProfile().token
                }
            }).then(function successCallback(response) {
                swangular.success("Användaren är nu bortagen");
                getUsers(1, 1);
            }, function errorCallback(response) {
                if (response.status === 401) {
                    swangular.alert('Du måste logga in igen');
                    vm.sent = false;
                    currentUser.removeProfile();
                    currentUser.getProfile();
                }
            });
        }

        vm.saveUser = function (user) {

            if (user.password === user.confirmPassword) {
                var listUrl = dataConstants.API_PATH + 'Admin/updateUser';
                http({
                    method: 'POST',
                    url: listUrl,
                    data: user,
                    headers: {
                        Authorization: 'Bearer ' + currentUser.getProfile().token
                    }
                }).then(function successCallback(response) {
                    $scope.hideUpdate = true;
                    swangular.success("Användaren är nu uppdaterad");
                    getUsers(1, 1);
                }, function errorCallback(response) {
                    if (response.status === 401) {
                        swangular.alert('Du måste logga in igen');
                        vm.sent = false;
                        currentUser.removeProfile();
                        currentUser.getProfile();
                    }
                    swangular.alert(response.data.message);
                });

            }
            else {
                swangular.alert('Lösenorden matchar inte');
            }
        }

        vm.setSettings = function (settings) {
            //setSettings
            var listUrl = dataConstants.API_PATH + 'Admin/setSettings';
            http({
                method: 'POST',
                url: listUrl,
                data: settings,
                headers: {
                    Authorization: 'Bearer ' + currentUser.getProfile().token
                }
            }).then(function successCallback(response) {
                swangular.success("Användaren är nu uppdaterad");
                getUsers(1, 1);
            }, function errorCallback(response) {
                if (response.status === 401) {
                    swangular.alert('Du måste logga in igen');
                    vm.sent = false;
                    currentUser.removeProfile();
                    currentUser.getProfile();
                }
            });
        }

        vm.getSettings = function () {
            var listUrl = dataConstants.API_PATH + 'Admin/getSettings';
            http({
                method: 'GET',
                url: listUrl,
                headers: {
                    Authorization: 'Bearer ' + currentUser.getProfile().token
                }
            }).then(function successCallback(response) {
                vm.smtp = response.data.smtp;
                vm.settings.smtpHost = vm.smtp.host;
                vm.settings.smtpPort = vm.smtp.port;
                vm.settings.smtpUsername = vm.smtp.username;
                vm.settings.smtpPassword = vm.smtp.password;

                vm.sql = response.data.sql;
                vm.settings.sqlHost = vm.sql.host;
                vm.settings.sqlPort = vm.sql.port;
                vm.settings.sqlDatabase = vm.sql.database;
                vm.settings.sqlUsername = vm.sql.username;
                vm.settings.sqlPassword = vm.sql.password;

                console.log(vm.smtp);
                console.log(vm.sql);
            }, function errorCallback(response) {
                if (response.status === 401) {
                    swangular.alert('Du måste logga in igen');
                    vm.sent = false;
                    currentUser.removeProfile();
                    currentUser.getProfile();
                }
            });
        }

        vm.addNewUser = function (user) {
            if (user.password === user.confirmPassword) {
                var listUrl = dataConstants.API_PATH + 'Account/Register';
                http({
                    method: 'POST',
                    url: listUrl,
                    data: user,
                    headers: {
                        Authorization: 'Bearer ' + currentUser.getProfile().token
                    }
                }).then(function successCallback(response) {
                    swangular.success("Användaren är nu tillagd");
                    getUsers(1, 1);
                }, function errorCallback(response) {
                    if (response.status === 401) {
                        swangular.alert('Du måste logga in igen');
                        vm.sent = false;
                        currentUser.removeProfile();
                        currentUser.getProfile();
                    }
                });
                $scope.hideNewUser = true;
            }
            else {
                swangular.alert('Lösenorden matchar inte');
            }
        }
        
        vm.newUser = function () {
            vm.userData.name = '';
            vm.userData.email = '';
            vm.userData.password = '';
            vm.userData.confirmPassword = '';
            $scope.hideNewUser = false;
            $scope.hideUpdate = true;
        }

        vm.closeUpdate = function (user) {
            vm.userData.name = '';
            vm.userData.email = '';
            vm.userData.password = '';
            vm.userData.confirmPassword = '';
            $scope.hideUpdate = true;
            $scope.hideNewUser = true;
        }

        function getUsers(pageSize, pageNumber) {
            var listUrl = dataConstants.API_PATH + 'Admin/getallUsers/' + pageSize + '/' + pageNumber;
            http({
                method: 'GET',
                url: listUrl,
                headers: {
                    Authorization: 'Bearer ' + currentUser.getProfile().token
                }
            }).then(function successCallback(response) {
                vm.json = response.data;
                vm.users = vm.json.users;
                vm.total = vm.json.totalCount;
                console.log(vm.users);
            }, function errorCallback(response) {
                if (response.status === 401) {
                    swangular.alert('Du måste logga in igen');
                    vm.sent = false;
                    currentUser.removeProfile();
                    currentUser.getProfile();
                }
            });
        }

        $scope.pageChanged = function () {

            var listUrl = dataConstants.API_PATH + 'roles/getallUsers/' + vm.pageSize + '/' + vm.pageNumber;
            http({
                method: 'GET',
                url: listUrl
            }).then(function successCallback(response) {
                vm.json = response.data;
                vm.users = vm.json.users;
                vm.total = vm.json.totalCount;
            }, function errorCallback(response) {
                vm.error = response;
            });
            // http.get(url).then(onUsersSucess, onFailure);
        };

        vm.userAdmin = function () {
            var role = currentUser.getProfile().roles;
            if (role.length !== undefined) {
                if (role.includes('Admin')) {
                    return false;
                } else {
                    return true;
                }
            } else {
                return true;
            }
        }

        vm.openTab = function (evt, tabName) {
            var i, tabcontent, tablinks;

            tabcontent = document.getElementsByClassName("tabcontent");
            for (i = 0; i < tabcontent.length; i++) {
                tabcontent[i].style.display = "none";
            }

            tablinks = document.getElementsByClassName("tablinks");
            for (i = 0; i < tablinks.length; i++) {
                tablinks[i].className = tablinks[i].className.replace(" active", "");
            }

            document.getElementById(tabName).style.display = "block";
            evt.currentTarget.className += " active";
            $scope.hideUpdate = true;
        }

        vm.isLoggedIn = function () {
            if (currentUser.getProfile().isLoggedIn) {
                if (vm.userAdmin() === false) {
                    return true;
                } else {
                    return false;
                }
            } else {
                return false;
            }
        };

        function activate() {
            getUsers(1, 1);
        }

    }
})();