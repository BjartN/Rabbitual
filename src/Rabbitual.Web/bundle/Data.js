/// <reference path="../typings/tsd.d.ts"/>
var DataService = (function () {
    function DataService() {
        this.root = 'http://localhost:9000';
    }
    DataService.prototype.getAgentOptions = function (agentId, callback) {
        this.get(this.root + "/agent/options/" + agentId, callback);
    };
    DataService.prototype.getConfig = function (callback) {
        var that = this;
        this.get(this.root + "/config", function (data) {
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
    };
    DataService.prototype.getSchema = function (agentId, callback) {
        this.get((this.root + "/agent/options/schema/") + agentId, callback);
    };
    DataService.prototype.getFatOptions = function (agentId, callback) {
        this.get((this.root + "/agent/fat-options/") + agentId, callback);
    };
    DataService.prototype.getOptions = function (agentId, callback) {
        this.get((this.root + "/agent/options/") + agentId, callback);
    };
    DataService.prototype.getAgentTypes = function (callback) {
        $.ajax({
            type: 'GET',
            url: this.root + "/agent/types",
            success: callback
        });
    };
    DataService.prototype.postOptions = function (agentId, value, callback) {
        this.post((this.root + "/agent/options/update/") + agentId, JSON.stringify(value), callback);
    };
    DataService.prototype.postConfig = function (agentId, value, callback) {
        this.post((this.root + "/agent/config/update/") + agentId, JSON.stringify(value), callback);
    };
    DataService.prototype.postAgent = function (name, type, callback) {
        this.post(this.root + "/agent-create", { name: name, type: type }, callback);
    };
    DataService.prototype.post = function (url, data, callback) {
        $.ajax({
            type: 'POST',
            data: data,
            url: url,
            success: callback
        });
    };
    DataService.prototype.get = function (url, callback) {
        $.ajax({
            type: 'GET',
            url: url,
            success: callback
        });
    };
    return DataService;
}());
