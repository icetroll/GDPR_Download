(function () {
    'use strict';

    var app = angular.module('app');

    app.directive('modal', function () {
        return {
            templateUrl: 'app/sendModal/send.html',
            controller: 'SendController',
            controllerAs: 'vm',
            restrict: 'E',
            transclude: true,
            replace: true,
            scope: true,
            link: function postLink(scope, element, attrs) {
                scope.title = attrs.title;
                scope.fileName = scope.$parent.fileName;

                scope.$watch(attrs.visible, function (value) {
                    if (value === true)
                        $(element).modal('show');
                    else
                        $(element).modal('hide');
                });

                $(element).on('shown.bs.modal', function () {
                    scope.$apply(function () {
                        scope.$parent[attrs.visible] = true;
                    });
                });

                $(element).on('hidden.bs.modal', function () {
                    scope.$apply(function () {
                        scope.$parent[attrs.visible] = false;
                    });
                });
            }
        };
    });
    
    app.directive('fileUpload', function () {
        return {
            scope: true,
            link: function (scope, el, attrs) {
                el.bind('change', function (event) {
                    var files = event.target.files;
                    //iterate files since 'multiple' may be specified on the element
                    if (files.length === 0) {
                        scope.$emit("fileSelected", { file: null, field: event.target.name });
                    } else {
                        for (var i = 0; i < files.length; i++) {
                            //emit event upward
                            scope.$emit("fileSelected", { file: files[i], field: event.target.name });
                        }
                    }
                });
            }
        };
    });
})();