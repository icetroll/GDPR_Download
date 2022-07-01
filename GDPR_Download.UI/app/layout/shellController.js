(function () {
    'use strict';

    var controllerId = 'ShellController';
    angular
        .module('app')
        .controller(controllerId, ['$location', '$rootScope', 'common', shell]);

    function shell($location, $rootScope, common) {
        var vm = this;
        vm.title = 'shell';
        activate();

        function activate() {
            common.activateController([], controllerId);
        }
    }
})();
