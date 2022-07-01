(function () {
    'use strict';

    angular.module('app').factory('alerts', ['$timeout', alerts]);

    function alerts($timeout) {
        var currentAlerts = [];
        var alertTypes = ['info', 'warning', 'success', 'danger'];

        function addInfo(message) {
            addAlert('info', message);
        }

        function addWarning(message) {
            addAlert('warning', message);
        }

        function addSuccess(message) {
            addAlert('success', message);
        }

        function addDanger(message) {
            addAlert('danger', message);
        }

        function addAlert(type, message) {
            $('html,body').scrollTop(0);
            var alert = { type: type, message: message };
            currentAlerts.push(alert);

            $timeout(function () {
                removeAlert(alert);
            }, 5000);
        }

        function removeAlert(alert) {
            for (var i = 0; i < currentAlerts.length; i++) {
                if (currentAlerts[i] === alert) {
                    currentAlerts.splice(i, 1);
                    break;
                }
            }
        }

        return {
            removeAlert: removeAlert,
            addAlert: addAlert,
            addInfo: addInfo,
            addWarning: addWarning,
            currentAlerts: currentAlerts,
            alertTypes: alertTypes,
            addSuccess: addSuccess,
            addDanger: addDanger
        }
    }
})();