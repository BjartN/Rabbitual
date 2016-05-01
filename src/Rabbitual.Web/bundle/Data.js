/// <reference path="../typings/jquery/jquery.d.ts"/>
/// <reference path="../typings/whatwg-fetch/whatwg-fetch.d.ts"/>
/// <reference path="../typings/underscore/underscore.d.ts"/>
var DataService = (function () {
    function DataService() {
        this.root = 'http://localhost:9000';
    }
    DataService.prototype.get = function (callback) {
        var that = this;
        fetch(this.root + "/config").then(function (r) {
            r.json().then(function (data) {
                var props = ['lastCheck', 'lastEventOut', 'lastEventIn', 'lastTaskIn', 'lastTaskOut'];
                _.each(data, function (x) {
                    _.each(props, function (p) {
                        if (x[p]) {
                            x[p] = moment.utc(x[p]).fromNow();
                        }
                    });
                });
                callback(data);
            });
        });
    };
    DataService.prototype.getSchema = function (agentId, callback) {
        fetch((this.root + "/agent/options/schema/") + agentId).then(function (r) {
            r.json().then(function (data) {
                callback(data);
            });
        });
    };
    DataService.prototype.getOptions = function (agentId, callback) {
        fetch((this.root + "/agent/options/") + agentId).then(function (r) {
            r.json().then(function (data) {
                callback(data);
            });
        });
    };
    DataService.prototype.postAgent = function (name, type, callback) {
        $.ajax({
            type: 'POST',
            data: { name: name, type: type },
            url: this.root + "/agent-create",
            success: callback
        });
    };
    DataService.prototype.getAgentTypes = function (callback) {
        $.ajax({
            type: 'GET',
            url: this.root + "/agent/types",
            success: callback
        });
    };
    DataService.prototype.postOptions = function (agentId, value, callback) {
        var bodz = JSON.stringify(value);
        $.ajax({
            type: 'POST',
            url: (this.root + "/agent/options/update/") + agentId,
            data: bodz,
            success: callback
        });
    };
    return DataService;
}());
