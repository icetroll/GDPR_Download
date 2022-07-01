(function () {
    'use strict';

    var controllerId = 'SendController';

    angular
        .module('app')
        .controller(controllerId, ['$scope', '$http', 'userAccount', 'currentUser', 'swangular', 'dataConstants', sendController]);

    function sendController($scope, $http, userAccount, currentUser, swangular, dataConstants) {
        var vm = this;
        vm.title = 'SendController';
        vm.fileName = '';
        vm.sent = false;

        vm.sendData = {
            reciverEmail: '',
            subject: '',
            description: '',
            fileName: '',
            sent: true
        };

        activate();

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

        vm.send = function () {
            vm.sent = true;
            vm.sendData.fileName = $scope.$parent.fileName;

            //var inData = { 'uploadInfo': vm.sendData };

            var inData = vm.sendData;

            $http({
                method: 'POST',
                url: dataConstants.API_PATH + "Upload/send",
                data: inData,
                headers: {
                    'Authorization':
                        function () {
                            return 'Bearer ' + currentUser.getProfile().token;
                        }
                }
            }).then(function successCallback(response) {
                if (response.data === true) {
                    swangular.success('Din fil har nu skickats', { timer: 4000 });
                    $scope.$parent.showModal = !$scope.$parent.showModal;
                    vm.sent = false;
                } else {
                    swangular.alert("Något gick fel din fil har inte skickats", { timer: 4000 });
                    vm.sent = false;
                }
            }, function errorCallback(response) {
                if (response.status === 401) {
                    swangular.alert("Du måste logga in igen", { timer: 4000 });
                    vm.sent = false;
                    currentUser.removeProfile();
                    currentUser.getProfile();
                } else {
                    swangular.alert("Något gick fel din fil har inte skickats", { timer: 4000 });
                    vm.sent = false;
                }
            });
        }

        vm.isLoggedIn = function () {
            if (currentUser.getProfile().isLoggedIn) {
                return true;
            } else {
                return false;
            }
        };

        function activate() {
        }

    }
})();