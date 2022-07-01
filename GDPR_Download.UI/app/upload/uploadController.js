(function () {
    'use strict';

    var controllerId = 'UploadController';

    angular
        .module('app')
        .controller(controllerId, ['$scope', '$http', 'userAccount', 'currentUser', 'dataConstants', 'swangular', uploadController]);

    function uploadController($scope, $http, userAccount, currentUser, dataConstants, swangular) {
        var vm = this;
        vm.title = 'UploadController';
        vm.message = '';
        $scope.uploadFiles = [];
        $scope.uploadFilesInfo = [];
        $scope.formData = new FormData();
        $scope.showModal = false;

        activate();

        vm.files = function () {
            $scope.uploadFiles = $scope.uploadFiles;
        }


        vm.LoadFileData = function (files) {
            for (var file in files) {
                $scope.formData.append("file", files[file]);
            }
        };

        function updateFileList(files) {
            $scope.formData = new FormData();
            vm.LoadFileData(files);
            for (var file in files) {
                if (files.hasOwnProperty(file)) {
                    $scope.uploadFiles.push({ name: files[file].name });
                }
            }
        }

        vm.openModal = function () {
            $scope.showModal = false;
            $scope.showModal = !$scope.showModal;
        }

        vm.startUploadFiles = function () {
            var config = {
                headers: {
                    'Content-Type': 'multipart/form-data',
                }
            };

            var filePassword = document.getElementById("filePW").value;

            $http({
                method: 'POST',
                url: dataConstants.API_PATH + "Upload/Files/" + filePassword,
                data: $scope.formData,
                transformRequest: angular.identity,
                headers: { 
                    'Content-Type': undefined,
                    'Authorization':
                        function () {
                            return 'Bearer ' + currentUser.getProfile().token;
                        }
                }
            }).then(function successCallback(response) {
                $scope.fileName = response.data;
                $scope.showModal = !$scope.showModal;
            }, function errorCallback(response) {
                if (response.status === 401) {
                    swangular.alert("Du måste logga in igen", { timer: 4000 });
                    currentUser.removeProfile();
                    currentUser.getProfile();
                } else {
                    swangular.alert("Uppladdningen misslyckades", { timer: 4000 });
                }
            });
        }

        $('#uploadFiles').change(function () {
            updateFileList(this.files);
        })

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