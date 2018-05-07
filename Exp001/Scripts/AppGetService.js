angular.module('AppService', []).service('Get', function ($http) {
    var service = {}
    service.GetAdres = function (pagingInfo) {
        return $http.get('/GetUsers/Get', { params: pagingInfo });
    };
    return service;
})