var DataService = (function () {
    function DataService() {
    }
    DataService.prototype.get = function (callback) {
        var that = this;
        fetch('http://localhost:9000/config').then(function (r) {
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
        fetch('http://localhost:9000/agent/options/schema/' + agentId).then(function (r) {
            r.json().then(function (data) {
                callback(data);
            });
        });
    };
    DataService.prototype.getOptions = function (agentId, callback) {
        fetch('http://localhost:9000/agent/options/' + agentId).then(function (r) {
            r.json().then(function (data) {
                callback(data);
            });
        });
    };
    DataService.prototype.postOptions = function (agentId, value, callback) {
        fetch("http://localhost:9000/agent/update/" + agentId, {
            method: "POST",
            body: value
        })
            .then(function (res) { return res.json(); })
            .then(function (data) { callback(data); });
    };
    return DataService;
}());
